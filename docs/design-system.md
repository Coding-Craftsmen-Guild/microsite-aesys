# Aesys BESS — Design System / Token Spec

> Extracted from the Figma desktop file (10 desktop page mockups, ~1463 px wide each) via MCP `get_design_context` reading literal values from the generated reference code. **No Figma variables/styles are defined in the file** — every token below is a measured raw value, reconciled across frames.
>
> **Scope caveat:** all mockups are **desktop-only (~1463 px)**. Mobile/tablet breakpoints, hover/focus/active/disabled states, and motion are **NOT in the design** — see [§9 Gaps](#9-gaps--unknowns). Treat those as decisions to make, not facts to extract.
>
> Pages covered: Homepage, Proizvodi (listing + 2 product details), Usluge/Podrška, O nama, Kontakt, Blog list, Blog single, Testimonials list.

---

## 1. Foundations

### 1.1 Type family

| Token | Value |
|---|---|
| `--font-family-base` | **Red Hat Display**, sans-serif |

Weights in use (map Figma weight name → CSS `font-weight`):

| Figma name | `font-weight` | Where |
|---|---|---|
| Regular | `400` | body, nav links, footer links, card body, meta, tag label |
| SemiBold | `600` | eyebrow labels (`— LABEL`) |
| Bold | `700` | section/footer headers, active nav, button labels, pill CTAs, inline emphasis |
| ExtraBold | `800` | section headings (44), card/step titles (19), stat numbers (42), pull-quotes, "ISTAKNUT" lead |
| Black | `900` | hero/blog H1 (62), numbered chips, 01–06 grid numerals, "Od jednog kabineta do 100+ MW", primary-button label "Zatražite besplatnu analizu" |

> Single typeface, no secondary/mono. Load weights 400/600/700/800/900.

### 1.2 Color palette

```
// Brand
--color-navy            #0E0E2C   // primary dark — page bg on dark pages, dark cards/chips, text on light
--color-accent          #A3B440   // the single accent (olive-lime green) — headings accent words,
                                   //   numerals, links/arrows, buttons, borders. NO second accent.

// Surfaces
--color-surface-dark    #0E0E2C   // = navy (dark sections, dark cards)
--color-surface-light   #F8F8F8   // ALL light cards, form panel, inputs' container. No separate gray.
--color-white           #FFFFFF   // input fields, outline-button fill, text on dark
--color-black           #000000   // hero base layer only (behind hero image)

// Lines
--color-divider         #D9D9D9   // 1px hairline dividers (header, footer, section separators)

// Text
--color-text-on-dark    #FFFFFF
--color-text-on-light   #0E0E2C   // navy used as body text on light surfaces
--color-text-accent     #A3B440

// Overlays
--color-scrim-navy      rgba(14,14,44,0.6)   // dark scrim over photo bands (hero, CTA bands)
```

> ⚠️ **Reconciliation notes**
> - There is exactly **one** accent (`#A3B440`). The "multi-color heading" seen on Proizvodi is a single accent applied to multiple words.
> - There is **no distinct light-gray** token — `#F8F8F8` is reused for every light card and the form.
> - "Borders" on cards are frequently declared as `rgba(163,180,64,0)` (fully transparent green) or `border:0` — i.e. **cards are borderless by default**; the reserved green border is presumably a selected/hover affordance (unconfirmed).

### 1.3 Radii

| Token | Value | Used on |
|---|---|---|
| `--radius-sm` | `6px` | small icon tick chip (26×26) |
| `--radius-card` | `15px` | all cards, form panel, inputs, textarea |
| `--radius-chip` | `20px` | numbered step / roman-numeral chips (81×81) |
| `--radius-image` | `22px` | feature/content photos (full-rounded images) |
| `--radius-image-top` | `15px` top-only | card top images (`border-radius: 15px 15px 0 0`) |
| `--radius-pill` | `200px` | all buttons, pills, lang toggle, tags |

### 1.4 Effects

| Token | Value | Notes |
|---|---|---|
| `--scrim` | `rgba(14,14,44,0.6)` | overlay on photo CTA/hero bands |
| shadow | **none** | no drop shadows are declared on cards/buttons in the file |
| decorative watermark | large faint Aesys "leaf" glyph behind some sections (e.g. contact form, roman-list) | low-opacity brand mark, decorative only |

---

## 2. Typography scale

Sorted by size. Line-height (LH) and weight are the measured defaults per role.

| role | size | LH | weight | case | color | usage |
|---|---|---|---|---|---|---|
| `display / hero-h1` | **62px** | 69px | Black 900 (Bold 700 on home) | as-authored | white (navy/white) + accent word | Homepage hero, Blog single H1 (green, uppercase) |
| `heading / section-h2` | **44px** | 70px | ExtraBold 800 | sentence | navy on light / white on dark, accent word `#A3B440` | every section heading, page H1s (non-hero), pull-quotes |
| `stat-number` | **42px** | 76px | ExtraBold 800 | — | white | homepage stats row only |
| `numeral / grid-number` | **44px** | 28px | Black 900 | — | `#A3B440` | "01–06" big numerals |
| `numeral / chip` | **29px** | — | Black 900 | — | white on filled / navy on outline | step & roman chips |
| `body-lg / subhead` | **19px** | 28px | Regular 400 | — | white on dark / navy on light | hero subhead, intros, article body, meta, list items |
| `title / card` | **19px** | 28px | ExtraBold 800 | — | navy on light / white on dark | card titles, step titles, "ISTAKNUT" lead (19/28 ExtraBold white) |
| `label / inline-strong` | **19px** | — | Black 900 | — | navy + accent | "Od jednog kabineta do 100+ MW" |
| `body / base` | **14px** | 20px | Regular 400 | — | per surface | card body, footer, form labels, copyright |
| `body / list` | **14px** | 28–35px | Regular 400 | — | per surface | checklists, hero bullet list (looser LH) |
| `eyebrow / label` | **14px** | (76px box) | SemiBold 600 | UPPERCASE | white/navy | `— SECTION LABEL` with leading dash |
| `nav-link` | **14px** | normal | Regular 400 (active **Bold 700 + accent**) | — | white | header nav |
| `button-label` | **14px** | normal | Bold 700 (some Black 900) | — | white / navy+accent | all buttons & pills |
| `footer-header` | **14px** | 20px | Bold 700 | UPPERCASE | white | STRANICE / KONTAKT / BRZI LINKOVI |
| `meta / date` | **14px** | 20px | Regular 400 | — | navy | blog card date "01.01.2001." |
| `tag-label` | **14px** | normal | Regular 400 | — | `#A3B440` | Tag pills |
| `footnote` | **9px** | 20px | Regular 400 | — | white | "*Procene zavise…" |

**Distinct font sizes:** `9, 14, 19, 29, 42, 44, 62`.

> Practical scale to expose as tokens: `xs:9 · sm:14 · md:19 · chip:29 · stat:42 · h2:44 · h1:62`.

---

## 3. Spacing & layout

### 3.1 Page grid

| Token | Value |
|---|---|
| `--page-width` (mockup) | ~1463 px |
| `--content-max-width` | **1159 px** (≈ `1160`) |
| `--page-gutter` (side padding) | **~156 px** each side (content x ≈ 157→1316) |
| inter-column gap | **19 px** (cards), used in both 3-col and 4-col grids |

> Container pattern: `max-width: 1160px; margin-inline: auto;` with section bands going **full-bleed** (dark/photo bands span 100% width; content inside stays at 1160).

### 3.2 Grid systems

| Grid | Columns | Card width | Gap | Pages |
|---|---|---|---|---|
| 3-col cards | 3 | 370px | 19px | Blog list, Testimonials, "Zašto investirati", "Oblasti ekspertize" |
| 4-col cards | 4 | 276px | 19px | homepage implementation steps, Usluge feature cards |
| 2-col products | 2 | 561px | ~29px | Proizvodi listing |
| 3-col footer | 3 (+ brand col) | — | columns at x ≈ 688 / 902 / 1116 | every page |
| 8-cell logo grid | 4×2 | 97px cells | ~41px | homepage clients |

### 3.3 Spacing scale (observed px)

```
2  (1.997 stat divider width)
4  (chip border, top rail thickness 3–4)
19 (grid column gap; near 20)
26 (small icon chip)
28 (line-height unit)
36 (input height)
41 (button/pill height)
57 (icon-circle badge, small)
77 / 81 (icon-circle large / square chip)
96 (roman/step list row pitch)
~156 (page gutter)
```

> Most values cluster on a **~4/8 base grid**, with `19px` gaps being the notable near-20 off-grid value (consistent everywhere, so treat `19` as the canonical gutter, not a 16/24 step). Recommend normalizing to a `4px` base scale: `4 · 8 · 16 · 20 · 24 · 28 · 36 · 41 · 57 · 80 · 96 · 156`.

### 3.4 Component heights

| Element | Height |
|---|---|
| input / select | **36px** |
| textarea | 93px |
| button / pill / lang toggle / tag | **41px** (30px for the small header pills & tags) |
| icon-tick chip | 26px |
| icon-circle badge | 57px (cards) / 77px (process columns) |
| numbered square chip | 81px |

---

## 4. Iconography

- **Style:** thin **line icons** (Untitled-UI / Hugeicons-style set — e.g. `lightning-01`, `key-01`, `layers-three-01`, `dataflow-04`, `shield-03`, `container`, `battery-full`, `monitor-01`, `git-branch-01`, `users-03`, `refresh-cw-01`).
- **Sizes:** 18px (inside 26px tick chip), 32–38px (inside 57px circle), 44–51px (inside 77px circle), 35–52px (bare on light feature cards).
- **Arrow:** the recurring `➤` (small right-pointing triangle) appears in **`#A3B440`** at the end of links/buttons. On links it sits inline; on solid green buttons it's white.
- **Icon-circle badge:** a filled ellipse (navy on dark cards) carrying a centered line icon, floating above the top edge of a card.

---

## 5. Component specs

All buttons/pills: `height 41px; border-radius 200px; font 14px Bold; text-align center`.

### 5.1 Buttons & pills

| Component | bg | border | text | size | notes |
|---|---|---|---|---|---|
| **Primary (solid green)** | `#A3B440` | none | white, **Black 900** | h41, w~235 | "Zatražite besplatnu analizu" |
| **Green pill CTA** | `#A3B440` | none | white, **Bold 700** | h41, w~204 | band CTAs ("Pričajte sa AESYS timom", "CTA BUTTON") |
| **Outline (light)** | `#FFFFFF` | `1px solid #A3B440` | navy word + green `➤`, Bold | h41, w 134–178 | "Naša rešenja ➤", "Još ➤", "Naši proizvodi ➤" |
| **Lang toggle pill** | transparent | `1px solid #FFFFFF` | white, 14px | h30, w62 | header "SRB" |
| **Tag pill** | transparent | `1px solid #A3B440` | `#A3B440`, Regular | h30, w~86 | blog "◌ Tag1" |
| **Hero eyebrow pill** | transparent | `1px solid #A3B440` | `#A3B440`, Regular | h30, w~284 | "◌ BESS Implementacija…" |

### 5.2 Text link (`➤`)

`text 14px Regular` (navy on light / white on dark) + `➤` in `#A3B440`, with a short **`#A3B440` 1px underline bar** sitting ~20px below, width 64–153px. Examples: "Saznaj više", "Nastavi", "Skini upitnik", "Studije slučaja", "Pričaj sa našim timom".

### 5.3 Header / nav (identical every page)

- Full-width over dark/hero. Logo (glyph + "aesys" wordmark) at left ~157px.
- Nav links: `14px Regular white`, centered; **active = Bold 700 `#A3B440`**.
- Right cluster: outline lang pill ("SRB", h30/w62/r200) + **solid green pill** "Zakažite konsultacije ➤" (`#A3B440`, h30, w195, r200, Bold white).
- Hairline `#D9D9D9` 1px under the bar, width 1159px.

### 5.4 Footer (identical every page)

- Band `#0E0E2C`, height 389px, full-bleed.
- Brand column: logo (132×29) + "Pametna energija, za pametnu industriju." (Bold 14 white) + address (Regular 14 white).
- 3 link columns: header **Bold 14 UPPERCASE white**; links Regular 14 white, line-height 20.
  - STRANICE · KONTAKT · BRZI LINKOVI
- Hairline `#D9D9D9` 1px above bottom row.
- Bottom row: copyright (Regular 14 white, left) + language switch "SR · EN · RO" (Regular 14 white, right).

### 5.5 Cards

| Card | bg | radius | border | size | content |
|---|---|---|---|---|---|
| **Light feature card** | `#F8F8F8` | 15px all | none (transparent green reserved) | 370×177 / 370×240 | bare line icon (top-left), title 19 ExtraBold navy, body 14 navy |
| **Dark feature card** | `#0E0E2C` | **15px bottom-only** (square top) | none | 276×~210 | 4px white top rail, floating 57px icon-circle, title 19 ExtraBold white, body 14 white |
| **Process column card** | `#0E0E2C` | 15px bottom-only | none | 371×~297 | 4px white top rail, floating **77px** icon-circle, title 19 ExtraBold white, body 14 white |
| **White product card** | `#F8F8F8` | 15px | none, **no shadow** | 561×~532 | product render image, title 19 ExtraBold navy, body 14 navy, "Saznaj više ➤" |
| **Blog / testimonial card** | `#F8F8F8` | 15px | none | 370×~591 | top image (370×325, top-radius 15), then date/IME (14 navy), title 19 ExtraBold navy, excerpt 14 navy, "Nastavi ➤" |

### 5.6 Numbered / process indicators

- **Step circle (Kontakt process 01/02/03):** 77px outlined circle, numeral **44px ExtraBold navy**, connected by a horizontal line; sits above a navy step card (15px bottom-radius, 4px white top rail) with title 19 ExtraBold white + body 14 white.
- **Numbered square chip (roman I–VI / steps):** 81×81, `radius 20px`. Alternating **filled** (`#0E0E2C`, white numeral) and **outline** (`4px solid #A3B440`, navy numeral); numeral **29px Black**. Adjacent label 19px Regular navy.
- **Big grid numeral (01–06):** 44px Black `#A3B440`, paired with card title + body.

### 5.7 Contact form

- Panel: `#F8F8F8`, radius 15px, ~528×453.
- Header "Pošaljite poruku" 19px ExtraBold navy; helper 14px Regular navy.
- Field labels: 14px Regular navy ("Ime i prezime*", "Kompanija", "Email*", "Telefon*", "Poruka*").
- Inputs: `#FFFFFF`, **h36**, radius 15px, 1px transparent-green border (invisible until focus, presumably). Half-width 222px, full-width 461px. Textarea h93.
- Submit: **outline-white pill** (white bg, 1px `#A3B440`, h41, w461, r200), label "Pošaljite poruku ➤" Bold navy + green arrow.

### 5.8 Eyebrow label

`— LABEL`: a leading dash + uppercase label, 14px SemiBold, white on dark / navy on light. Precedes every section heading.

### 5.9 Stat block (homepage)

Row of 4 stats separated by 2px white vertical dividers (90px tall): number **42px ExtraBold white**, caption 14px Regular white (~194px wide).

### 5.10 Pull-quote

Centered, **44px ExtraBold**, line-height 70. Two variants: white (O-nama mission quote) and `#A3B440` ("NAGLASAK" on blog single). Sits on a dark or photo-scrim band.

---

## 6. Section / band patterns

| Pattern | Treatment |
|---|---|
| **Dark section** | `#0E0E2C` full-bleed, white text, accent-green heading word, content at 1160 max-width |
| **Light section** | white page bg, navy text, `#F8F8F8` cards |
| **Photo CTA band** | background photo (cube/energy render) + `rgba(14,14,44,0.6)` navy scrim, white Bold 44 heading, green pill CTA |
| **Hero** | `#000` base + photo + navy scrim; 62px Black/Bold H1 with accent word; eyebrow pill; subhead 19; bullet list; primary button |
| **Split section** | heading + body/links on one side, photo (radius 22) on the other |
| **Section separators** | `#D9D9D9` 1px hairline at 1159px width |

---

## 7. Proposed token export (CSS custom properties)

```css
:root {
  /* color */
  --color-navy: #0E0E2C;
  --color-accent: #A3B440;
  --color-surface-light: #F8F8F8;
  --color-white: #FFFFFF;
  --color-black: #000000;
  --color-divider: #D9D9D9;
  --color-text-on-dark: #FFFFFF;
  --color-text-on-light: #0E0E2C;
  --color-scrim: rgba(14,14,44,0.6);

  /* type */
  --font-base: "Red Hat Display", sans-serif;
  --fw-regular: 400; --fw-semibold: 600; --fw-bold: 700; --fw-extrabold: 800; --fw-black: 900;
  --fs-footnote: 9px;
  --fs-body: 14px;       /* lh 20 */
  --fs-body-lg: 19px;    /* lh 28 */
  --fs-chip: 29px;
  --fs-stat: 42px;       /* lh 76 */
  --fs-h2: 44px;         /* lh 70 */
  --fs-h1: 62px;         /* lh 69 */

  /* radius */
  --radius-sm: 6px;
  --radius-card: 15px;
  --radius-chip: 20px;
  --radius-image: 22px;
  --radius-pill: 200px;

  /* layout */
  --content-max: 1160px;
  --page-gutter: 156px;
  --grid-gap: 19px;

  /* sizing */
  --control-h-sm: 30px;   /* small pills/tags */
  --control-h-input: 36px;
  --control-h-btn: 41px;
}
```

> When wiring into the repo's Tailwind v4 `@theme` (see `Aesys.Web/Client/tokens/tokens.scss`), these map to `--color-*`, `--font-*`, `--radius-*` and a custom spacing scale. The accent already has a Tailwind family (`bg-aesys-100` is referenced in CLAUDE.md) — confirm/extend the `aesys` color ramp to include `#A3B440` and `#0E0E2C`.

---

## 8. Page inventory (for reference)

| Frame | Page | Key sections |
|---|---|---|
| 1 (`2314:53`) | Homepage | hero + stats, "Kompletno rešenje", "Preuzmite kontrolu", "Zašto investirati" (01–06), "Karakteristike" (dark), clients, BESS solutions, "Bez prekida rada" CTA, contact form, footer |
| 2 (`2314:54`) | Kontakt | "Kontaktirajte nas" + checklist + map, process 01/02/03, "Razgovarajte sa našim timom" + form |
| 3 (`2314:55`) | O nama | "Budućnost koju gradimo", process columns (icon-circles), "Oblasti ekspertize" grid, "Energetski sektor", mission pull-quote |
| 4 (`2314:56`) | Usluge / Podrška | lifecycle hero, dark 4×2 feature grid, roman I–VI service list, contact form |
| 5 (`2314:57`) | Proizvodi | "BESS rešenja…", 2 product cards, "Rešite svoj energetski problem" CTA band |
| 6 (`2314:58`) | Kabinetska rešenja | Luminary-379/407 detail blocks + spec lists |
| 7 (`2314:59`) | Kontejnerska rešenja | Luminary-3790/4070 detail blocks |
| 8 (`2314:60`) | Blog list | "Saznajte više" + 3×2 cards + "Još" pagination |
| 9 (`2314:61`) | Blog single | tags, 62px green H1, lead, body, "NAGLASAK" pull-quote, CTA band |
| 10 (`2314:62`) | Testimonials list | "Saznajte više od naših klijenata" + 3 client cards + "Još" |

---

## 9. Gaps & unknowns

These are **not in the Figma file** and must be decided during build:

1. **Responsive / breakpoints** — only ~1463px desktop exists. No mobile/tablet layouts, no breakpoint tokens, no stacking rules. **UNKNOWN.**
2. **Interaction states** — no hover, focus, active, or disabled variants are designed. The transparent green borders on cards (`rgba(163,180,64,0)`) and inputs *hint* at a green selected/focus state, but it's unconfirmed.
3. **Shadows/elevation** — none declared; cards are flat. Confirm whether flat is intended or shadows were just not modeled.
4. **Motion** — no transition/animation specs (the testimonial "carousel" and process line imply motion but none is specified).
5. **Exact letter-spacing** — reference code reports `leading` but no explicit `letter-spacing`; assume `normal` unless design says otherwise.
6. **The `19px` gutter** — consistent but off the 4/8 grid; decide whether to keep `19` or normalize to `16`/`20`.
7. **Active-nav weight inconsistency** — home uses Black/900 for active "Početna"; other pages use Bold/700. Standardize on **Bold 700 + `#A3B440`**.
8. **Spelling** — Figma has both "expertize" (O-nama process heading) and "ekspertize" (grid). Copy decision, not design.
9. **Font hosting** — Red Hat Display is Google Fonts; confirm self-host vs CDN and the exact weight subset (400/600/700/800/900).
