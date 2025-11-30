# Urban Scum - Google Play Store Anleitung

## 📱 App-Beschreibung (Kurz - 80 Zeichen)
Springe, sammle & kämpfe dich durch 3 Levels! Süchtig machendes Arcade-Spiel!

## 📝 App-Beschreibung (Lang - 4000 Zeichen max)

🎮 **URBAN SCUM - Das ultimative Mobile-Plattformspiel!**

Tauche ein in die verrückte Welt von Urban Scum - einem süchtig machenden Arcade-Plattformspiel im coolen Cartoon-Style! Springe über Hindernisse, besiege fiese Gegner und sammle so viele Münzen wie möglich!

**🌟 FEATURES:**

✅ **3 Abwechslungsreiche Level**
- DOWNTOWN: Springe durch die Straßen der Innenstadt
- INDUSTRIAL: Überlebe die gefährliche Industriezone
- NIGHTLIFE: Entdecke das pulsierende Nachtleben

✅ **Süchtig machender Spielspaß**
- Einfache Touch-Steuerung für jeden
- Schnelles, actionreiches Gameplay
- Combo-System für Highscore-Jagd

✅ **Sammle & Verbessere**
- Goldmünzen und wertvolle Edelsteine
- Power-Ups: Speed Boost, Schild, Magnet, Doppelpunkte
- Steigere deinen Highscore!

✅ **Cooles Cartoon-Design**
- Bunte, moderne Grafiken
- Lustige Charaktere und Animationen
- Atmosphärische Level-Hintergründe

✅ **Speichere deinen Fortschritt**
- Level-Fortschritt wird automatisch gespeichert
- Highscore-Tabelle
- Sterne für jedes Level

**🎯 WIE MAN SPIELT:**
- Tippe links/rechts zum Bewegen
- Tippe auf den Blitz zum Springen
- Springe auf Feinde, um sie zu besiegen
- Sammle Münzen und Power-Ups
- Erreiche das Ende jedes Levels!

**⚠️ ACHTUNG:**
- Meide Stacheln und Hindernisse
- Pass auf die patrouillierenden Gegner auf
- Fallende Fässer sind gefährlich!

Lade jetzt Urban Scum herunter und werde der König der Straße! 🏆

---

## 🔑 Keywords (bis zu 500 Zeichen)

Plattformer, Jump and Run, Arcade, Casual Game, Endless Runner, Action, Adventure, Cartoon Spiel, Mobile Game, Offline Spiel, Gratis Spiel, Highscore, Coin Collector, Side Scroller, Retro Game, Fun Game, Quick Game, Family Game, Kids Game, Skill Game

## 📊 Kategorisierung

- **Hauptkategorie:** Spiele
- **Unterkategorie:** Arcade
- **Altersfreigabe:** PEGI 3 / USK 0 (Keine Gewalt, keine unangemessenen Inhalte)
- **Enthält Werbung:** Ja (Interstitial und Belohnungswerbung)
- **In-App-Käufe:** Nein

---

## 🖼️ Screenshot-Anforderungen

Du benötigst mindestens 2 Screenshots für den Play Store.

### Empfohlene Screenshots:
1. **Startbildschirm** - Zeigt den Hauptmenü mit Titel
2. **Gameplay Level 1** - Action im Downtown-Level
3. **Gameplay Level 2** - Industriegebiet
4. **Gameplay Level 3** - Nachtleben-Level
5. **Level-Auswahl** - Zeigt alle 3 Level
6. **Game Over** - Highscore-Anzeige
7. **Power-Up in Aktion** - Spieler mit Schild

### Screenshot-Spezifikationen:
- **Format:** PNG oder JPEG
- **Auflösung:** Mindestens 320px, max 3840px
- **Seitenverhältnis:** 16:9 (Landscape) oder 9:16 (Portrait)
- **Empfohlen:** 1080x1920 (Portrait)

---

## 🎨 Grafik-Assets

### App-Icon
- **Größe:** 512x512 Pixel
- **Format:** 32-Bit PNG (mit Alpha)
- **Kein Rand** (Google fügt automatisch Schatten hinzu)

### Feature-Grafik (Banner)
- **Größe:** 1024x500 Pixel
- **Format:** PNG oder JPEG (kein Alpha)
- **Tipp:** Zeige das Logo groß und zentral

---

## 📋 Upload-Anleitung (Schritt für Schritt)

### Voraussetzungen:
1. Google Play Developer Account ($25 einmalig)
2. Signierter Android App Bundle (.aab)
3. Alle Screenshots und Grafiken
4. Datenschutzerklärung (URL)

### Schritt 1: App erstellen
1. Gehe zu [Google Play Console](https://play.google.com/console)
2. Klicke auf "App erstellen"
3. Wähle Standard-App
4. Gib den App-Namen ein: "Urban Scum"
5. Wähle Sprache: Deutsch
6. App-Typ: Spiel
7. Kostenlos oder Kostenpflichtig: Kostenlos
8. Akzeptiere die Nutzungsbedingungen

### Schritt 2: App-Inhalte einrichten
1. **Datenschutzrichtlinie:** URL deiner Datenschutzerklärung
2. **App-Zugriff:** Alle Funktionen ohne Login verfügbar
3. **Anzeigen:** App enthält Anzeigen (Ja)
4. **Inhaltsbewertung:** Fragebogen ausfüllen (IARC)
5. **Zielgruppe:** Über 13 Jahre (wegen Werbung)
6. **News-App:** Nein
7. **COVID-19 Kontakt-App:** Nein
8. **Datensicherheit:** Ausfüllen (keine persönlichen Daten)

### Schritt 3: Store-Eintrag einrichten
1. Kurzbeschreibung einfügen
2. Vollständige Beschreibung einfügen
3. App-Icon hochladen
4. Feature-Grafik hochladen
5. Screenshots hochladen (min. 2)
6. Kategorie auswählen: Spiele > Arcade
7. Tags hinzufügen
8. Kontakt-E-Mail angeben

### Schritt 4: App-Bundle hochladen
1. Gehe zu "Produktion" > "Releases"
2. Klicke auf "Neues Release erstellen"
3. Lade die .aab-Datei hoch
4. Versionsnummer angeben (z.B. 1.0.0)
5. Release-Notizen eingeben

### Schritt 5: Überprüfung starten
1. Alle Checklisten-Punkte grün?
2. Klicke auf "Zur Überprüfung einreichen"
3. Warte auf Google-Überprüfung (1-7 Tage)

---

## 🔧 Technische Details

### Build-Anleitung:

```bash
# 1. Dependencies installieren
cd urban-scum
npm install

# 2. Capacitor initialisieren
npx cap init "Urban Scum" com.urbanscum.game --web-dir .

# 3. Android-Plattform hinzufügen
npx cap add android

# 4. Sync
npx cap sync

# 5. In Android Studio öffnen
npx cap open android

# 6. Release-Build erstellen (in Android Studio)
# Build > Generate Signed Bundle / APK > Android App Bundle
```

### AdMob-Integration:
1. Erstelle ein AdMob-Konto
2. Füge eine neue App hinzu
3. Erstelle Interstitial und Rewarded Ad Units
4. Ersetze die Test-IDs in `src/js/ads.js` mit echten IDs
5. Ersetze die App-ID in `capacitor.config.json`

---

## ✅ Checkliste vor dem Upload

- [ ] App-Icon (512x512) erstellt
- [ ] Feature-Grafik (1024x500) erstellt
- [ ] Mindestens 2 Screenshots
- [ ] Datenschutzerklärung online
- [ ] AdMob-IDs eingefügt (Produktion)
- [ ] App signiert (.aab Bundle)
- [ ] Alle Texte Korrektur gelesen
- [ ] App auf echtem Gerät getestet
- [ ] Inhaltsbewertung ausgefüllt
- [ ] Kontaktdaten angegeben

---

## 📞 Support

Bei Fragen zur Veröffentlichung:
- [Google Play Console Hilfe](https://support.google.com/googleplay/android-developer)
- [Capacitor Dokumentation](https://capacitorjs.com/docs)
- [AdMob Hilfe](https://support.google.com/admob)

Viel Erfolg mit Urban Scum! 🎮🚀
