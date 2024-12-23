# 📚 Projectstructuur
Dit project bestaat uit drie hoofdonderdelen: **ModelLibrary**, **RealDolmenAPI** en **RealDolmenIntenum**. Hieronder volgt een gedetailleerde uitleg van de structuur en het doel van elk onderdeel en de inhoud ervan.

# 📁 ModelLibrary
Het ModelLibrary-project bevat modellen, view modellen en Data Transfer Objects (DTO’s) die gedeeld worden door de applicatie. Het dient als een centrale plaats voor het definiëren van de datastructuren die door de andere projecten worden gebruikt.

**Structuur**

Dto: Bevat Data Transfer Objects, die worden gebruikt om gegevens tussen processen over te dragen.

Models: Bevat de modelklassen die de kerngegevensstructuren vertegenwoordigen.

ViewModels: Bevat de view modellen die in de applicatie worden gebruikt.

# 🌐 RealDolmenAPI
Het RealDolmenAPI-project is de backend API die gegevens en diensten aan de applicatie levert. Het bevat controllers, services en configuraties die nodig zijn voor de werking van de API.

**Structuur**

Dépendances: Afhankelijkheden die door het project worden gebruikt.

Properties: Bevat projecteigenschappen en -instellingen.

Controllers: Bevat API controllers die HTTP-verzoeken afhandelen en antwoorden teruggeven.

Data: Bevat data access en database-gerelateerde bestanden.

Error: Bevat error handling en logging bestanden.

Migrations: Bevat database migratiebestanden.

Services: Bevat serviceklassen die de bedrijfslogica implementeren.

appsettings.json: Configuratiebestand voor applicatie-instellingen.

Program.cs: Het hoofdingangspunt voor de applicatie.


# 💻 RealDolmenIntenum
Het RealDolmenIntenum-project is frontend-gedeelte van de applicatie. Het bevat componenten, pagina’s en andere resources die nodig zijn voor de gebruikersinterface.

**Structuur**

Connected Services: Bevat verbonden services die door de applicatie worden gebruikt.

Dépendances: Afhankelijkheden die door het project worden gebruikt.

Properties: Bevat projecteigenschappen en -instellingen.

wwwroot: Bevat statische bestanden en resources.

Components: Bevat herbruikbare UI-componenten.

Layout: Bevat layout-componenten voor de applicatie.

Pages: Bevat Razor-pagina’s die in de applicatie worden gebruikt.


# 🗃️ DataBank
Het DataBank-project bevat een script voor onze database en een afbeelding van het ERD (Entity-Relationship Diagram).

**Structuur**

Database script: Het script voor het opzetten van de database.

ERD afbeelding: Een afbeelding van het Entity-Relationship Diagram die de database structuur visualiseert.

#  📄 Documentatie
Het Documentatie-project bevat een korte documentatie over de applicatie.

**Structuur**

UserStory.docx: Een document dat de gebruikersverhalen en requirements van de applicatie beschrijft.
