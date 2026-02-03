# Test Analysis

## 1. Úvod

Cílem této testovací analýzy je navrhnout rizikově orientovaný testovací přístup pro demo e-shop s elektronikou, který umožňuje zákazníkům procházet produkty, vytvářet objednávky se slevami a administrátorům spravovat produkty prostřednictvím admin panelu.

Analýza je rozdělena do čtyř kroků podle obchodního a bezpečnostního dopadu:
- ochrana tržeb a plateb
- ochrana dat a konzistence systému
- stabilita uživatelského toku
- edge cases, bezpečnost a rozšiřující scénáře

Hlavní důraz je kladen na:
- finanční výpočty
- slevovou logiku
- objednávkový proces
- administraci

Chyby v těchto oblastech mohou vést k finanční ztrátě, poškození dat nebo narušení důvěry uživatelů.

---

## 2. Kroková prioritizace testování podle rizika

### KROK 1 – Kritické obchodní a administrační funkce

#### Objednávkový proces (Košík → Objednávka → Potvrzení)

Klíčová funkce celého systému s přímým dopadem na tržby.

**Rizika:**
- nesprávný výpočet ceny
- ztráta nebo změna dat mezi kroky
- rozdíl mezi zobrazenou a potvrzenou částkou

#### Slevy a cenová logika

Různé typy slev (věk, akce, platba kartou).

**Obchodní pravidla:**
- pouze jedna sleva najednou
- výjimka: sleva za platbu kartou

**Rizika:**
- nesprávná kombinace slev
- podúčtování nebo přeúčtování zákazníka
- porušení obchodních pravidel

#### Platby

Výběr platební metody a správná aplikace slevy za platbu kartou.

Nutná konzistence mezi:
- cenou v košíku
- cenou v objednávce
- skutečně účtovanou částkou

#### Admin panel – správa produktů a cen

Přístup pouze přes dedikovanou URL s autentizací administrátora.

**CRUD operace:**
- vytváření produktů
- změna ceny
- změna skladového množství
- mazání produktů

**Rizika:**
- manipulace s cenou
- neautorizovaný přístup
- finanční ztráta
- poškození katalogu produktů

---

### KROK 2 – Ochrana dat a konzistence systému

#### Konzistence dat

Shoda mezi adminem a e-shopem, mezi košíkem, objednávkou a potvrzením.

**Rizika:**
- neaktuální data
- nesoulad mezi administrací a frontendem
- chyby obtížně detekovatelné zákazníkem

#### Autorizace a oprávnění

Oddělení rolí (admin vs zákazník) a nemožnost přístupu k admin funkcím bez přihlášení.

---

### KROK 3 – Uživatelský tok a stabilita systému

#### Košík
- přidávání a odebírání produktů
- změna množství
- přepočet ceny v reálném čase  
- stavové přechody: prázdný → plný košík

#### Detail produktu
- správné zobrazení názvu
- správná cena
- dostupnost produktu
- kontrola skladových zásob

#### Formulář objednávky
- povinná pole
- validace e-mailu a telefonu
- kontrola délky vstupů  
- prevence neúplných nebo neplatných objednávek

---

### KROK 4 – Edge cases, bezpečnost a doplňky

#### Edge cases
- extrémní množství produktů v košíku
- neobvyklé kombinace dopravy a platby
- hraniční a neplatné vstupy

#### Základní bezpečnostní kontroly
- přístup do admin panelu bez přihlášení
- manipulace s cenou na frontendu
- opakované odeslání objednávky
- přímý přístup na chráněné URL

#### Doplňkové funkce
- exporty
- vizuální vzhled UI
- responzivita
- explorativní testování nových funkcí

---

## 3. Testovací techniky a úrovně testování

- **Objednávka a slevy** – integrační / E2E  
  Rozhodovací tabulky, E2E scénáře
- **Košík** – integrační  
  Boundary value analysis, stavové testy
- **Formuláře** – systémové  
  Ekvivalence tříd, negativní testy
- **Admin panel** – systémové  
  CRUD, autorizace, persistence
- **Edge cases** – explorativní  
  Negativní a ad-hoc testy

---

## 4. Strategie automatizace testování

### Automatizovat prioritně (Krok 1 + 2)
- E2E objednávkový proces
- výpočty slev a konečné ceny
- validaci objednávkového formuláře
- základní scénáře admin panelu
- ověření změn z pohledu zákazníka

### Neautomatizovat hned
- vizuální kontrolu UI
- responzivitu
- explorativní testování
- edge cases a bezpečnostní scénáře

---

## 5. Závěr

Navržený testovací přístup je rizikově orientovaný a zaměřuje se na ochranu tržeb, správnost finančních výpočtů, integritu objednávek a bezpečnost administrace.

Testování postupuje od nejkritičtějších scénářů k méně rizikovým, což umožňuje rychle dosáhnout vysoké úrovně kvality bez zbytečné režie.

Automatizace je doporučena zejména pro obchodně kritické a stabilní scénáře, aby byla zajištěna spolehlivá regresní kontrola.
