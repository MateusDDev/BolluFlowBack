import math
import random
import subprocess
import sys
from collections import defaultdict
from datetime import date, datetime, timedelta, timezone
from decimal import ROUND_HALF_UP, Decimal

CONTAINER = "bolluflow-db"
DB_USER = "user"
DB_NAME = "BolluFlow"

EMAIL = "demo@doceencanto.test"
PASSWORD = "Doce@Demo2026"
COMPANY = "Doce Encanto Confeitaria"
SLUG = "doce-encanto"

START = date(2025, 12, 1)
TODAY = date(2026, 11, 20)
NOW_UTC = datetime(2026, 11, 20, 20, 30, tzinfo=timezone.utc)
LOCAL = timezone(timedelta(hours=-3))

random.seed(42)


def psql(sql, db=DB_NAME):
    result = subprocess.run(
        ["docker", "exec", "-i", CONTAINER, "psql", "-U", DB_USER, "-d", db, "-v", "ON_ERROR_STOP=1", "-At", "-q"],
        input=sql.encode("utf-8"),
        capture_output=True,
    )
    if result.returncode != 0:
        sys.stderr.write(result.stderr.decode("utf-8", "replace"))
        raise SystemExit(result.returncode)
    return result.stdout.decode("utf-8").strip()


def money(value):
    return Decimal(value).quantize(Decimal("0.01"), rounding=ROUND_HALF_UP)


def lit(value):
    if value is None:
        return "NULL"
    if isinstance(value, bool):
        return "TRUE" if value else "FALSE"
    if isinstance(value, (int, Decimal)):
        return str(value)
    if isinstance(value, float):
        return repr(round(value, 6))
    if isinstance(value, datetime):
        return "'" + value.astimezone(timezone.utc).strftime("%Y-%m-%d %H:%M:%S+00") + "'"
    return "'" + str(value).replace("'", "''") + "'"


def local_dt(day, hour, minute=0):
    return datetime(day.year, day.month, day.day, hour, minute, tzinfo=LOCAL)


class Ids:
    def __init__(self, table):
        self.next = int(psql(f"select coalesce(max(id),0) from {table}")) + 1

    def take(self):
        value = self.next
        self.next += 1
        return value


rows = defaultdict(list)


def insert(table, **values):
    rows[table].append(values)


if psql(f"select count(*) from tb_user where email = {lit(EMAIL)} or slug = {lit(SLUG)}") != "0":
    print("Demo user already exists. Remove it first or change EMAIL/SLUG.")
    raise SystemExit(1)

password_hash = psql(f"create extension if not exists pgcrypto; select crypt({lit(PASSWORD)}, gen_salt('bf', 11));", db="postgres")

ids = {t: Ids(t) for t in [
    "tb_user", "tb_customer", "tb_delivery_address", "tb_ingredient", "tb_ingredient_conversion",
    "tb_supply", "tb_supply_conversion", "tb_recipe", "tb_recipe_conversion", "tb_product",
    "tb_product_component", "tb_order", "tb_order_item", "tb_financial_entries",
    "tb_stock_movements", "tb_recipe_productions",
]}

user_id = ids["tb_user"].take()
user_created = local_dt(date(2025, 11, 25), 9, 12)
insert("tb_user", id=user_id, email=EMAIL, password=password_hash, name=COMPANY, profile_image_url=None,
       created_at=user_created, indirect_cost_rate=Decimal("12"), whatsapp_message_template=None, is_active=True,
       slug=SLUG, whatsapp="11987452310", allow_negative_stock=False, enabled_modules=15,
       onboarding_completed_at=user_created + timedelta(minutes=20))

INGREDIENTS = [
    ("leite_condensado", "Leite condensado", "Gram", "0.0203", 4740, 2000, [("Lata 395g", 395)]),
    ("creme_leite", "Creme de leite", "Gram", "0.0190", 2400, 800, [("Caixinha 200g", 200)]),
    ("choc_meio", "Chocolate meio amargo", "Gram", "0.0520", 2000, 1000, [("Barra 1kg", 1000)]),
    ("choc_branco", "Chocolate branco", "Gram", "0.0580", 1000, 500, [("Barra 1kg", 1000)]),
    ("cacau", "Cacau em pó 50%", "Gram", "0.0750", 500, 200, [("Pacote 500g", 500)]),
    ("manteiga", "Manteiga sem sal", "Gram", "0.0520", 1000, 500, [("Tablete 200g", 200)]),
    ("acucar", "Açúcar refinado", "Gram", "0.0049", 5000, 2000, [("Pacote 1kg", 1000)]),
    ("farinha", "Farinha de trigo", "Gram", "0.0058", 5000, 2000, [("Pacote 1kg", 1000)]),
    ("ovos", "Ovos", "Unit", "0.7500", 30, 12, [("Dúzia", 12), ("Bandeja 30", 30)]),
    ("leite", "Leite integral", "Milliliter", "0.0055", 6000, 2000, [("Caixa 1L", 1000)]),
    ("granulado", "Granulado de chocolate", "Gram", "0.0380", 1000, 300, [("Pacote 1kg", 1000)]),
    ("limao", "Limão tahiti", "Unit", "0.4500", 20, 6, []),
    ("morango", "Morango", "Gram", "0.0280", 1000, None, [("Bandeja 250g", 250)]),
    ("cream_cheese", "Cream cheese", "Gram", "0.0460", 1200, 400, [("Pote 300g", 300)]),
    ("leite_po", "Leite em pó", "Gram", "0.0620", 800, 200, [("Pacote 400g", 400)]),
    ("corante", "Corante vermelho em gel", "Milliliter", "0.3000", 25, None, []),
    ("fermento", "Fermento químico", "Gram", "0.0300", 250, 50, []),
]

SUPPLIES = [
    ("pote", "Pote 250ml com tampa", "0.95", 50, 20, [("Pacote 50 un", 50)]),
    ("forminha", "Forminha nº 4", "0.06", 500, 100, [("Pacote 100 un", 100)]),
    ("caixa4", "Caixa para 4 doces", "1.40", 50, 15, []),
    ("caixa12", "Caixa para 12 doces", "2.80", 25, 8, []),
    ("caixa_bolo", "Caixa para bolo 25cm", "4.20", 10, 4, []),
    ("base_bolo", "Base laminada 25cm", "1.80", 10, 4, []),
    ("emb_ovo", "Embalagem para ovo de Páscoa", "3.50", 30, None, []),
    ("celofane", "Saquinho celofane", "0.25", 100, 30, []),
    ("fita", "Fita de cetim (metro)", "0.40", 50, 10, []),
    ("emb_cone", "Embalagem para cone", "0.35", 100, 20, []),
    ("caixa_panetone", "Caixa para panetone", "3.90", 20, None, []),
    ("caixa_presente", "Caixa presente kraft", "6.50", 10, 3, []),
]

ingredient = {}
for key, name, unit, cost, pkg, minimum, conversions in INGREDIENTS:
    iid = ids["tb_ingredient"].take()
    ingredient[key] = dict(id=iid, name=name, unit=unit, cost=Decimal(cost), pkg=pkg, min=minimum, stock=Decimal(0))
    for cname, qty in conversions:
        insert("tb_ingredient_conversion", id=ids["tb_ingredient_conversion"].take(), ingredient_id=iid, name=cname,
               quantity_in_base_unit=Decimal(qty))

supply = {}
for key, name, cost, pkg, minimum, conversions in SUPPLIES:
    sid = ids["tb_supply"].take()
    supply[key] = dict(id=sid, name=name, unit="Unit", cost=Decimal(cost), pkg=pkg, min=minimum, stock=Decimal(0))
    for cname, qty in conversions:
        insert("tb_supply_conversion", id=ids["tb_supply_conversion"].take(), supply_id=sid, name=cname,
               quantity_in_base_unit=Decimal(qty))

RECIPES = [
    ("brigadeiro", "Massa de brigadeiro", "Recheios", 40, 450,
     [("leite_condensado", 395), ("manteiga", 15), ("cacau", 20), ("creme_leite", 50)]),
    ("brigadeiro_branco", "Brigadeiro de ninho", "Recheios", 40, 460,
     [("leite_condensado", 395), ("manteiga", 15), ("leite_po", 30), ("creme_leite", 50)]),
    ("massa_choc", "Massa de chocolate", "Massas", 60, 1300,
     [("farinha", 300), ("acucar", 280), ("cacau", 80), ("ovos", 4), ("leite", 240), ("manteiga", 120), ("fermento", 15)]),
    ("massa_branca", "Massa branca", "Massas", 55, 1250,
     [("farinha", 320), ("acucar", 260), ("ovos", 5), ("leite", 240), ("manteiga", 120), ("fermento", 15)]),
    ("ganache", "Ganache meio amargo", "Coberturas", 20, 580, [("choc_meio", 400), ("creme_leite", 200)]),
    ("limao", "Recheio de limão", "Recheios", 15, 640, [("leite_condensado", 395), ("creme_leite", 200), ("limao", 4)]),
    ("red_velvet", "Massa red velvet", "Massas", 60, 1250,
     [("farinha", 300), ("acucar", 280), ("ovos", 3), ("leite", 240), ("manteiga", 120), ("cacau", 15), ("corante", 10), ("fermento", 12)]),
    ("cream_cheese", "Cobertura de cream cheese", "Coberturas", 15, 560, [("cream_cheese", 300), ("manteiga", 80), ("acucar", 200)]),
    ("brownie", "Massa de brownie", "Massas", 50, 1000,
     [("choc_meio", 300), ("manteiga", 200), ("acucar", 300), ("ovos", 4), ("farinha", 120), ("cacau", 30)]),
    ("cookie", "Massa de cookie", "Massas", 35, 900,
     [("farinha", 300), ("manteiga", 150), ("acucar", 200), ("ovos", 2), ("choc_meio", 200), ("fermento", 5)]),
]

recipe = {}
for key, name, category, prep, yield_g, items in RECIPES:
    rid = ids["tb_recipe"].take()
    batch_cost = sum(ingredient[i]["cost"] * q for i, q in items)
    recipe[key] = dict(id=rid, name=name, yield_=Decimal(yield_g), items=items, cost_per_g=batch_cost / yield_g)
    insert("tb_recipe", id=rid, user_id=user_id, name=name, category=category, preparation_time=prep, base_unit="Gram",
           final_price=Decimal(0), stock_quantity=Decimal(0), minimum_stock_quantity=None, is_active=True,
           yield_=Decimal(yield_g), track_stock=False)
    for i, q in items:
        insert("tb_recipe_ingredient", recipe_id=rid, ingredient_id=ingredient[i]["id"], quantity_used=Decimal(q))
    insert("tb_recipe_conversion", id=ids["tb_recipe_conversion"].take(), recipe_id=rid, name="Receita inteira",
           quantity_in_base_unit=Decimal(yield_g))

R, I, S = "Recipe", "Ingredient", "Supply"
PRODUCTS = [
    ("pote_brig", "Bolo de pote de brigadeiro", "Bolos de pote", "13.00", True, "Massa de chocolate fofinha com muito brigadeiro.",
     [(R, "massa_choc", 90), (R, "brigadeiro", 80), (S, "pote", 1)]),
    ("pote_ninho", "Bolo de pote ninho com morango", "Bolos de pote", "14.00", True, "Massa branca, brigadeiro de ninho e morangos frescos.",
     [(R, "massa_branca", 90), (R, "brigadeiro_branco", 80), (I, "morango", 30), (S, "pote", 1)]),
    ("brig_un", "Brigadeiro gourmet", "Docinhos", "3.50", True, "Brigadeiro de chocolate 50% cacau.",
     [(R, "brigadeiro", 20), (I, "granulado", 3), (S, "forminha", 1)]),
    ("caixa4", "Caixa com 4 brigadeiros", "Docinhos", "15.00", True, "Ótima para presentear.",
     [(R, "brigadeiro", 80), (I, "granulado", 12), (S, "forminha", 4), (S, "caixa4", 1)]),
    ("caixa12", "Caixa com 12 docinhos sortidos", "Docinhos", "42.00", True, "Brigadeiro tradicional e de ninho.",
     [(R, "brigadeiro", 120), (R, "brigadeiro_branco", 120), (I, "granulado", 18), (S, "forminha", 12), (S, "caixa12", 1), (S, "fita", 0.5)]),
    ("cento", "Cento de docinhos para festa", "Festa", "130.00", True, "100 docinhos sortidos. Encomenda com 3 dias de antecedência.",
     [(R, "brigadeiro", 1000), (R, "brigadeiro_branco", 1000), (I, "granulado", 150), (S, "forminha", 100)]),
    ("brownie", "Brownie", "Individuais", "9.00", True, "Brownie de chocolate meio amargo com casquinha.",
     [(R, "brownie", 90), (S, "celofane", 1)]),
    ("cookie", "Cookie com gotas de chocolate", "Individuais", "8.00", True, None,
     [(R, "cookie", 90), (S, "celofane", 1)]),
    ("cone", "Cone trufado", "Individuais", "11.00", True, "Casquinha de chocolate recheada com ganache.",
     [(R, "ganache", 60), (I, "choc_meio", 40), (S, "emb_cone", 1)]),
    ("bolo_choc", "Bolo de chocolate (1,5 kg)", "Bolos", "95.00", True, "Massa de chocolate, brigadeiro e cobertura de ganache.",
     [(R, "massa_choc", 900), (R, "brigadeiro", 450), (R, "ganache", 200), (S, "caixa_bolo", 1), (S, "base_bolo", 1)]),
    ("red_velvet", "Bolo red velvet (1,5 kg)", "Bolos", "115.00", True, "Com cobertura de cream cheese e frutas vermelhas.",
     [(R, "red_velvet", 900), (R, "cream_cheese", 560), (I, "morango", 150), (S, "caixa_bolo", 1), (S, "base_bolo", 1)]),
    ("ninho_morango", "Bolo ninho com morango (2 kg)", "Bolos", "140.00", True, "O queridinho das festas de aniversário.",
     [(R, "massa_branca", 1200), (R, "brigadeiro_branco", 900), (I, "morango", 400), (S, "caixa_bolo", 1), (S, "base_bolo", 1)]),
    ("torta_limao", "Torta de limão", "Tortas", "75.00", True, None,
     [(R, "massa_branca", 400), (R, "limao", 640), (S, "caixa_bolo", 1), (S, "base_bolo", 1)]),
    ("panetone", "Panetone trufado (700 g)", "Natal", "59.00", True, "Massa artesanal recheada com ganache. Encomendas até 20/12.",
     [(I, "farinha", 250), (I, "acucar", 60), (I, "ovos", 3), (I, "manteiga", 80), (I, "leite", 80), (I, "choc_meio", 120), (R, "ganache", 150), (S, "caixa_panetone", 1)]),
    ("ovo_trufado", "Ovo de Páscoa trufado 350 g", "Páscoa", "69.00", False, None,
     [(I, "choc_meio", 250), (R, "ganache", 120), (S, "emb_ovo", 1)]),
    ("ovo_colher", "Ovo de colher de brigadeiro 500 g", "Páscoa", "89.00", False, None,
     [(I, "choc_meio", 300), (R, "brigadeiro", 200), (I, "granulado", 20), (S, "emb_ovo", 1)]),
    ("kit_maes", "Kit Dia das Mães", "Datas especiais", "89.00", False, "6 docinhos, brownie e cookie em caixa presente.",
     [(R, "brigadeiro", 120), (R, "brownie", 90), (R, "cookie", 90), (S, "forminha", 6), (S, "caixa_presente", 1), (S, "fita", 1)]),
    ("caixa_coracao", "Caixa coração Dia dos Namorados", "Datas especiais", "65.00", False, None,
     [(R, "brigadeiro", 160), (R, "brigadeiro_branco", 80), (I, "choc_branco", 50), (S, "forminha", 12), (S, "caixa_presente", 1)]),
    ("kit_pais", "Caixa de brownies Dia dos Pais", "Datas especiais", "49.00", False, "4 brownies em caixa presente.",
     [(R, "brownie", 360), (S, "caixa4", 1), (S, "fita", 1)]),
    ("kit_criancas", "Kit festinha Dia das Crianças", "Datas especiais", "55.00", False, None,
     [(R, "brigadeiro", 200), (R, "massa_branca", 300), (I, "granulado", 30), (S, "caixa12", 1)]),
]

product = {}
for key, name, category, price, active, description, components in PRODUCTS:
    pid = ids["tb_product"].take()
    cost = Decimal(0)
    for ctype, ckey, qty in components:
        qty = Decimal(str(qty))
        if ctype == R:
            src, unit_price, unit = recipe[ckey], recipe[ckey]["cost_per_g"], "Gram"
        elif ctype == I:
            src = ingredient[ckey]
            unit_price, unit = src["cost"], src["unit"]
        else:
            src = supply[ckey]
            unit_price, unit = src["cost"], "Unit"
        cost += unit_price * qty
        insert("tb_product_component", id=ids["tb_product_component"].take(), product_id=pid, component_type=ctype,
               recipe_id=src["id"] if ctype == R else None, ingredient_id=src["id"] if ctype == I else None,
               supply_id=src["id"] if ctype == S else None, name=src["name"], base_unit=unit,
               unit_price=money(unit_price) if ctype != R else unit_price.quantize(Decimal("0.0001")), quantity_used=qty)
    product[key] = dict(id=pid, name=name, price=Decimal(price), cost=money(cost), components=components)
    insert("tb_product", id=pid, user_id=user_id, name=name, sale_price=Decimal(price), stock_quantity=Decimal(0),
           minimum_stock_quantity=None, indirect_cost_percentage=None, markup_percentage=Decimal(0),
           final_price=Decimal(price), discount_percentage=None, is_active=active, category=category, image_url=None,
           description=description, track_stock=False)

FIRST = ["Ana", "Beatriz", "Camila", "Daniela", "Eduarda", "Fernanda", "Gabriela", "Helena", "Isabela", "Juliana",
         "Larissa", "Mariana", "Natália", "Patrícia", "Renata", "Sabrina", "Tatiane", "Vanessa", "Bruno", "Carlos",
         "Diego", "Felipe", "Gustavo", "Henrique", "Lucas", "Marcelo", "Rafael", "Rodrigo", "Thiago", "Vinícius",
         "Aline", "Cláudia", "Débora", "Luciana", "Priscila", "Simone", "Paulo", "André"]
LAST = ["Silva", "Santos", "Oliveira", "Souza", "Lima", "Pereira", "Costa", "Ferreira", "Almeida", "Ribeiro",
        "Carvalho", "Gomes", "Martins", "Rocha", "Barbosa", "Mendes", "Nascimento", "Araújo", "Cardoso", "Teixeira"]
STREETS = ["Rua das Flores", "Rua dos Ipês", "Av. Brasil", "Rua São José", "Rua Sete de Setembro", "Rua Dom Pedro II",
           "Rua das Palmeiras", "Av. Paulista", "Rua XV de Novembro", "Rua Tiradentes", "Rua das Acácias", "Rua Bela Vista"]
HOODS = ["Centro", "Jardim América", "Vila Nova", "Santa Cecília", "Bela Vista", "Jardim Primavera", "Vila Mariana", "Parque das Árvores"]
PREFS = [None, None, None, "Prefere entrega à tarde", "Ama brigadeiro de ninho", "Não gosta de coco",
         "Cliente fiel, sempre pede bolo de pote", "Intolerante a lactose: avisar antes", "Pede para escrever no topo do bolo",
         "Retira no balcão", "Paga sempre no Pix"]

customers = []
used_names = set()
for n in range(42):
    while True:
        name = f"{random.choice(FIRST)} {random.choice(LAST)}"
        if name not in used_names:
            used_names.add(name)
            break
    cid = ids["tb_customer"].take()
    birthday = None
    if random.random() < 0.55:
        birthday = datetime(random.randint(1968, 2003), random.randint(1, 12), random.randint(1, 28), tzinfo=timezone.utc)
    if n < 2:
        birthday = datetime(1990 + n * 4, 11, 24 + n, tzinfo=timezone.utc)
    insert("tb_customer", id=cid, user_id=user_id, name=name, whatsapp=f"119{random.randint(10000000, 99999999)}",
           preferences=random.choice(PREFS), birthday=birthday)
    if random.random() < 0.6:
        insert("tb_delivery_address", id=ids["tb_delivery_address"].take(), street=random.choice(STREETS),
               number=random.randint(12, 1980), neighborhood=random.choice(HOODS), customer_id=cid)
    customers.append(dict(id=cid, weight=random.choice([1, 1, 1, 2, 3, 6])))


def pick_customer():
    return random.choices(customers, weights=[c["weight"] for c in customers])[0]["id"]


MONTH_TARGET = {
    (2025, 12): 4300, (2026, 1): 1450, (2026, 2): 1550, (2026, 3): 2150, (2026, 4): 3900, (2026, 5): 3300,
    (2026, 6): 2800, (2026, 7): 1750, (2026, 8): 2400, (2026, 9): 1950, (2026, 10): 2500, (2026, 11): 1700,
}

SEASONS = [
    ("panetone", date(2025, 12, 8), date(2025, 12, 24), 1500, (1, 3), "Natal"),
    ("ovo_trufado", date(2026, 3, 22), date(2026, 3, 31), 650, (1, 3), "Páscoa"),
    ("ovo_colher", date(2026, 3, 22), date(2026, 3, 31), 450, (1, 2), "Páscoa"),
    ("ovo_trufado", date(2026, 4, 1), date(2026, 4, 4), 1300, (1, 3), "Páscoa"),
    ("ovo_colher", date(2026, 4, 1), date(2026, 4, 4), 900, (1, 2), "Páscoa"),
    ("kit_maes", date(2026, 5, 4), date(2026, 5, 10), 1300, (1, 2), "Mães"),
    ("caixa_coracao", date(2026, 6, 6), date(2026, 6, 12), 850, (1, 1), "Namorados"),
    ("kit_pais", date(2026, 8, 3), date(2026, 8, 9), 600, (1, 2), "Pais"),
    ("kit_criancas", date(2026, 10, 5), date(2026, 10, 11), 550, (1, 2), "Crianças"),
]

DIRECT_MENU = [("pote_brig", 25), ("pote_ninho", 20), ("brig_un", 10), ("caixa4", 14), ("brownie", 14), ("cookie", 10), ("cone", 7)]
CUSTOM_MENU = [("bolo_choc", 22), ("ninho_morango", 20), ("red_velvet", 10), ("torta_limao", 12), ("caixa12", 18), ("cento", 18)]
CAKE_NOTES = [None, None, None, "Escrever 'Feliz aniversário!' no topo", "Retirada às 16h", "Sem morango na decoração",
              "Entregar na portaria", "Festa de 1 ano: decoração em tons de azul", "Velas por conta do cliente",
              "Ligar ao chegar", "Bolo para 20 pessoas"]
PAYMENTS = [("Pix", 55), ("Cash", 18), ("DebitCard", 15), ("CreditCard", 12)]


def weighted(options):
    return random.choices([o[0] for o in options], weights=[o[1] for o in options])[0]


def day_weight(day):
    return {0: 0.5, 1: 0.8, 2: 0.9, 3: 1.0, 4: 1.5, 5: 2.0, 6: 1.2}[day.weekday()]


def days_between(a, b):
    return [a + timedelta(days=i) for i in range((b - a).days + 1)]


orders = []


def order_total(o):
    subtotal = sum(q * product[p]["price"] for p, q in o["items"])
    discount = Decimal(0)
    if o["discount"] is not None:
        discount = subtotal * o["discount"] / 100 if o["discount_type"] == "Percentage" else o["discount"]
    return subtotal - discount + o["shipping"]


def new_direct(day, items=None):
    hour = random.randint(9, 17 if day == TODAY else 19)
    created = local_dt(day, hour, random.randint(0, 59))
    if items is None:
        items = {}
        for _ in range(random.choices([1, 2, 3], weights=[55, 32, 13])[0]):
            p = weighted(DIRECT_MENU)
            items[p] = items.get(p, 0) + (random.randint(2, 8) if p == "brig_un" else random.choices([1, 2, 3], weights=[65, 28, 7])[0])
    o = dict(type="DirectSale", status="Delivered", customer=pick_customer() if random.random() < 0.3 else None,
             created=created, delivery=None, delivered=created + timedelta(seconds=random.randint(2, 40)),
             payment=weighted(PAYMENTS), discount=None, discount_type=None, shipping=Decimal(0), note=None,
             items=list(items.items()))
    orders.append(o)
    return o


def new_custom(delivery_day, items, status="Delivered", lead=(2, 9), note=None):
    delivery = local_dt(delivery_day, random.choice([10, 11, 14, 15, 16, 17, 18]), random.choice([0, 0, 30]))
    created = delivery - timedelta(days=random.randint(*lead), hours=random.randint(0, 6), minutes=random.randint(0, 59))
    if created.date() < START - timedelta(days=4):
        created = local_dt(START, 9, 30)
    created = min(created, NOW_UTC.astimezone(LOCAL) - timedelta(minutes=random.randint(30, 600)))
    shipping = Decimal(random.choice([0, 0, 0, 8, 10, 12, 15]))
    discount, discount_type = None, None
    roll = random.random()
    if roll < 0.08:
        discount, discount_type = Decimal(random.choice([5, 10])), "Percentage"
    elif roll < 0.12:
        discount, discount_type = Decimal(10), "Fixed"
    o = dict(type="CustomOrder", status=status, customer=pick_customer(), created=created, delivery=delivery,
             delivered=delivery + timedelta(minutes=random.randint(-20, 40)) if status == "Delivered" else None,
             payment=weighted(PAYMENTS) if status in ("Delivered", "Ready") or random.random() < 0.5 else None,
             discount=discount, discount_type=discount_type, shipping=shipping,
             note=note if note is not None else random.choice(CAKE_NOTES), items=list(items.items()))
    orders.append(o)
    return o


revenue = defaultdict(Decimal)


def month_of(d):
    return (d.year, d.month)


for pkey, a, b, budget, qty_range, label in SEASONS:
    spent = Decimal(0)
    window = days_between(a, b)
    weights = [1 + i * 0.35 for i in range(len(window))]
    while spent < budget:
        day = random.choices(window, weights=weights)[0]
        items = {pkey: random.randint(*qty_range)}
        if random.random() < 0.25:
            items["caixa4"] = 1
        if random.random() < 0.45:
            o = new_direct(day, items)
        else:
            o = new_custom(day, items, lead=(1, 12), note=random.choice([None, None, "Presente: capricha no laço!", f"Encomenda de {label}"]))
        total = order_total(o)
        spent += total
        revenue[month_of(day)] += total

for (year, month), target in MONTH_TARGET.items():
    first = date(year, month, 1)
    last = (date(year + month // 12, month % 12 + 1, 1) - timedelta(days=1))
    last = min(last, TODAY)
    window = days_between(first, last)
    weights = [day_weight(d) for d in window]
    while revenue[(year, month)] < target:
        day = random.choices(window, weights=weights)[0]
        if day == TODAY or random.random() < 0.66:
            o = new_direct(day)
        else:
            items = {weighted(CUSTOM_MENU): 1}
            if random.random() < 0.2:
                items["caixa12"] = items.get("caixa12", 0) + 1
            if random.random() < 0.15:
                items["brig_un"] = random.choice([10, 20, 30])
            o = new_custom(day, items)
        revenue[(year, month)] += order_total(o)

for _ in range(3):
    new_direct(TODAY)

cancel_days = [date(2026, 1, 17), date(2026, 3, 14), date(2026, 5, 9), date(2026, 7, 25), date(2026, 9, 19), date(2026, 11, 7)]
for d in cancel_days:
    o = new_custom(d, {weighted(CUSTOM_MENU): 1}, status="Canceled", note="Cliente desistiu da festa")
    o["payment"] = None

upcoming = [
    (TODAY, "Ready", {"bolo_choc": 1}, "Retirada às 19h"),
    (TODAY + timedelta(days=1), "InProduction", {"ninho_morango": 1, "caixa12": 1}, "Festa de 30 anos"),
    (TODAY + timedelta(days=1), "InProduction", {"cento": 1}, None),
    (TODAY + timedelta(days=2), "Pending", {"red_velvet": 1}, "Escrever 'Parabéns, mãe!'"),
    (TODAY + timedelta(days=3), "Pending", {"torta_limao": 1}, None),
    (TODAY + timedelta(days=5), "Pending", {"cento": 2}, "Formatura, entregar no salão"),
    (TODAY + timedelta(days=7), "Pending", {"bolo_choc": 1, "brig_un": 30}, None),
    (date(2026, 12, 18), "Pending", {"panetone": 4}, "Presentes da equipe"),
    (date(2026, 12, 20), "Pending", {"panetone": 2}, None),
    (date(2026, 12, 22), "Pending", {"panetone": 6, "caixa12": 2}, "Confraternização da empresa"),
    (date(2026, 12, 23), "Pending", {"panetone": 1}, None),
]
for day, status, items, note in upcoming:
    o = new_custom(day, items, status=status, lead=(1, 6), note=note)
    if day == TODAY:
        o["delivery"] = local_dt(TODAY, 19, 0)

orders.sort(key=lambda o: o["created"])
number = 0
for o in orders:
    number += 1
    oid = ids["tb_order"].take()
    o["id"], o["number"] = oid, number
    insert("tb_order", id=oid, number=number, customer_id=o["customer"], status=o["status"], note=o["note"],
           delivery_date=o["delivery"], payment_method=o["payment"], discount=o["discount"],
           discount_type=o["discount_type"], shipping_fee=o["shipping"], created_at=o["created"],
           delivered_at=o["delivered"], user_id=user_id, type=o["type"])
    for p, q in o["items"]:
        insert("tb_order_item", id=ids["tb_order_item"].take(), order_id=oid, product_id=product[p]["id"], quantity=q,
               unit_price=product[p]["price"], unit_cost=product[p]["cost"], stock_debited=False)


def add_entry(kind, amount, description, origin, origin_id, event_key, when):
    insert("tb_financial_entries", id=ids["tb_financial_entries"].take(), type=kind, amount=money(amount),
           description=description, origin=origin, origin_id=origin_id, event_key=event_key, created_at=when,
           updated_at=when, user_id=user_id)


for o in orders:
    if o["status"] == "Delivered":
        description = f"Venda direta #{o['number']}" if o["type"] == "DirectSale" else f"Venda - Pedido #{o['number']}"
        add_entry("Income", order_total(o), description, "Order", o["id"], f"order_delivered:{o['id']}", o["delivered"])


def movement(item_type, item, kind, qty, unit_cost, notes, event_key, when):
    mid = ids["tb_stock_movements"].take()
    insert("tb_stock_movements", id=mid, item_type=item_type, item_id=item["id"], movement_type=kind, quantity=Decimal(qty),
           unit_cost=unit_cost, total_value=money(Decimal(qty) * unit_cost), notes=notes, event_key=event_key,
           created_at=when, updated_at=when)
    return mid


def buy(item_type, item, packages, when, initial=False):
    qty = Decimal(item["pkg"] * packages)
    unit_cost = item["cost"] * Decimal(str(random.choice([0.95, 1.0, 1.0, 1.04, 1.08])))
    unit_cost = unit_cost.quantize(Decimal("0.0001"))
    mid = movement(item_type, item, "Purchase", qty, unit_cost, None, None, when)
    if not initial:
        rows["tb_stock_movements"][-1]["event_key"] = f"stock_purchase:{mid}"
    origin = "IngredientPurchase" if item_type == "Ingredient" else "SupplyPurchase"
    description = f"Estoque inicial - {item['name']}" if initial else f"Compra - {item['name']}"
    add_entry("Expense", qty * unit_cost, description, origin, item["id"], None if initial else f"stock_purchase:{mid}", when)
    item["stock"] += qty


def ensure(item_type, item, needed, when):
    floor = Decimal(item["min"] or 0)
    if item["stock"] - needed >= floor:
        return
    missing = needed + floor - item["stock"]
    packages = max(1, math.ceil(missing / item["pkg"]))
    if needed > item["pkg"] * 3:
        packages += 1
    buy(item_type, item, packages, when - timedelta(days=random.randint(1, 2), hours=random.randint(0, 5)))


setup_time = user_created + timedelta(minutes=35)
for item in ingredient.values():
    buy("Ingredient", item, 2 if item["min"] else 1, setup_time, initial=True)
for item in supply.values():
    if item["min"]:
        buy("Supply", item, 1, setup_time, initial=True)

weeks = defaultdict(lambda: dict(recipes=defaultdict(Decimal), ingredients=defaultdict(Decimal), supplies=defaultdict(Decimal)))
for o in orders:
    if o["status"] not in ("Delivered", "Ready", "InProduction"):
        continue
    when = (o["delivery"] or o["created"]).astimezone(LOCAL).date()
    week_start = when - timedelta(days=when.weekday())
    bucket = weeks[week_start]
    for p, q in o["items"]:
        for ctype, ckey, qty in product[p]["components"]:
            total = Decimal(str(qty)) * q
            if ctype == R:
                bucket["recipes"][ckey] += total
            elif ctype == I:
                bucket["ingredients"][ckey] += total
            else:
                bucket["supplies"][ckey] += total

for week_start in sorted(weeks):
    bucket = weeks[week_start]
    prod_day = max(week_start, START)
    when = local_dt(prod_day, 8, random.randint(0, 50))
    for rkey, grams in sorted(bucket["recipes"].items()):
        rec = recipe[rkey]
        batches = Decimal(math.ceil(grams / rec["yield_"] * 2)) / 2
        for ikey, q in rec["items"]:
            ensure("Ingredient", ingredient[ikey], Decimal(q) * batches, when)
        pid = ids["tb_recipe_productions"].take()
        insert("tb_recipe_productions", id=pid, recipe_id=rec["id"], quantity=batches,
               notes=random.choice([None, None, "Produção da semana", "Lote para encomendas"]), created_at=when)
        for ikey, q in rec["items"]:
            need = Decimal(q) * batches
            ingredient[ikey]["stock"] -= need
            movement("Ingredient", ingredient[ikey], "Consumption", need, Decimal(0), f"Consumo - Produção de {rec['name']}",
                     f"recipe_production:{pid}:ingredient:{ingredient[ikey]['id']}", when)
        when += timedelta(minutes=random.randint(20, 70))
    week_end = min(week_start + timedelta(days=6), TODAY)
    close = local_dt(week_end, 20, 0) if week_end < TODAY else local_dt(TODAY, 16, 0)
    for ikey, qty in sorted(bucket["ingredients"].items()):
        ensure("Ingredient", ingredient[ikey], qty, close)
        ingredient[ikey]["stock"] -= qty
        movement("Ingredient", ingredient[ikey], "Consumption", qty, Decimal(0), "Consumo - Pedidos da semana", None, close)
    for skey, qty in sorted(bucket["supplies"].items()):
        ensure("Supply", supply[skey], qty, close)
        supply[skey]["stock"] -= qty
        movement("Supply", supply[skey], "Consumption", qty, Decimal(0), "Consumo - Pedidos da semana", None, close)

for key, qty in [("granulado", 520), ("cream_cheese", 650)]:
    item = ingredient[key]
    take = min(Decimal(qty), item["stock"])
    if take > 0:
        item["stock"] -= take
        movement("Ingredient", item, "Consumption", take, Decimal(0), "Consumo - Testes de novos sabores", None, local_dt(TODAY, 11, 0))
caixa = supply["caixa_bolo"]
if caixa["stock"] > 2:
    used = caixa["stock"] - 2
    caixa["stock"] = Decimal(2)
    movement("Supply", caixa, "Consumption", used, Decimal(0), "Consumo - Caixas avariadas", None, local_dt(TODAY, 11, 5))

FIXED = []
cursor = START
while cursor <= TODAY:
    y, m = cursor.year, cursor.month
    def at(d, h=10):
        return local_dt(date(y, m, min(d, 28)), h, random.randint(0, 59))
    if date(y, m, 20) <= TODAY:
        FIXED.append(("DAS MEI", Decimal("81.05"), at(20)))
    if date(y, m, 10) <= TODAY:
        FIXED.append(("Energia elétrica (cozinha)", Decimal(random.randint(78, 112)), at(10)))
        FIXED.append(("Internet e celular", Decimal("89.90"), at(8)))
    ads = 150 if m in (12, 3, 4, 5) else 60
    if date(y, m, 3) <= TODAY:
        FIXED.append(("Impulsionamento Instagram", Decimal(ads), at(3, 21)))
    cursor = date(y + m // 12, m % 12 + 1, 1)

gas_day = date(2025, 12, 6)
while gas_day <= TODAY:
    FIXED.append(("Gás de cozinha (P13)", Decimal(random.choice([118, 125, 130])), local_dt(gas_day, 15)))
    gas_day += timedelta(days=random.randint(32, 45))

FIXED += [
    ("Batedeira planetária nova", Decimal("649.00"), local_dt(date(2026, 3, 9), 14)),
    ("Formas e bicos de confeitar", Decimal("138.70"), local_dt(date(2026, 2, 18), 16)),
    ("Curso de confeitaria (ovos de colher)", Decimal("197.00"), local_dt(date(2026, 2, 25), 19)),
    ("Manutenção da geladeira", Decimal("220.00"), local_dt(date(2026, 7, 14), 11)),
]
for description, amount, when in FIXED:
    if when <= NOW_UTC:
        add_entry("Expense", amount, description, "Manual", None, None, when)
add_entry("Income", Decimal("150.00"), "Aula de brigadeiro gourmet (workshop)", "Manual", None, None, local_dt(date(2026, 9, 26), 18))

for key, name, unit, cost, pkg, minimum, conversions in INGREDIENTS:
    item = ingredient[key]
    insert("tb_ingredient", id=item["id"], user_id=user_id, name=name, base_unit=unit, unit_cost=item["cost"],
           stock_quantity=item["stock"], minimum_stock_quantity=Decimal(minimum) if minimum else None, track_stock=True)
for key, name, cost, pkg, minimum, conversions in SUPPLIES:
    item = supply[key]
    insert("tb_supply", id=item["id"], user_id=user_id, name=name, base_unit="Unit", unit_cost=item["cost"],
           stock_quantity=item["stock"], minimum_stock_quantity=Decimal(minimum) if minimum else None, track_stock=True)

ORDER = ["tb_user", "tb_customer", "tb_delivery_address", "tb_ingredient", "tb_ingredient_conversion", "tb_supply",
         "tb_supply_conversion", "tb_recipe", "tb_recipe_ingredient", "tb_recipe_conversion", "tb_product",
         "tb_product_component", "tb_order", "tb_order_item", "tb_financial_entries", "tb_stock_movements",
         "tb_recipe_productions"]

out = ["begin;"]
for table in ORDER:
    data = rows[table]
    if not data:
        continue
    cols = list(data[0].keys())
    col_sql = ", ".join('"yield"' if c == "yield_" else c for c in cols)
    for start in range(0, len(data), 500):
        chunk = data[start:start + 500]
        values = ",\n".join("(" + ", ".join(lit(r[c]) for c in cols) + ")" for r in chunk)
        out.append(f"insert into {table} ({col_sql}) values\n{values};")
for table in ORDER:
    if table == "tb_recipe_ingredient":
        continue
    out.append(f"select setval(pg_get_serial_sequence('{table}', 'id'), (select max(id) from {table}));")
out.append("commit;")

psql("\n".join(out))

delivered = [o for o in orders if o["status"] == "Delivered"]
print(f"user_id={user_id} email={EMAIL}")
print(f"orders={len(orders)} delivered={len(delivered)} customers={len(customers)}")
by_month = defaultdict(Decimal)
for o in delivered:
    by_month[o["delivered"].astimezone(LOCAL).strftime("%Y-%m")] += order_total(o)
for k in sorted(by_month):
    print(k, money(by_month[k]))
print("avg/month", money(sum(by_month.values()) / len(by_month)))
low = [i["name"] for i in list(ingredient.values()) + list(supply.values()) if i["min"] and i["stock"] < i["min"]]
print("below minimum:", ", ".join(low))
