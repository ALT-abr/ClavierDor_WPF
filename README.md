# Clavier D'Or

Quiz desktop en `C# / WPF` pour tester la culture technique de joueurs autour de plusieurs categories du developpement logiciel, avec systeme de pouvoirs, boss par categorie, boss final, historique des parties et export PDF.

## Apercu

**Clavier D'Or** est un jeu de quiz solo dans lequel le joueur :

- cree une nouvelle partie avec un nom et un pouvoir
- repond a une suite de questions techniques
- avance par categories dans un ordre defini
- affronte un boss a la fin de chaque categorie
- termine sur un boss final
- consulte l'historique de ses parties
- exporte un rapport PDF

Le projet utilise une architecture simple basee sur `WPF + MVVM + Entity Framework Core + MySQL/XAMPP`.

## Fonctionnalites

- page d'accueil immersive avec navigation vers les modules principaux
- creation de partie avec choix du joueur et du pouvoir
- reprise d'une partie non terminee
- quiz alimente depuis la base de donnees
- ordre des categories :
  - `anglais`
  - `culture_generale`
  - `metiers_informatique`
  - `logique`
  - `algorithmes`
- boss de categorie et boss final
- systeme de score :
  - question normale : `+3`
  - boss : `+30` si reussi, `-10` si echec
  - boss final : `+50` si reussi, `-20` si echec
  - score max : `500`
- pouvoirs utilisables une seule fois :
  - `Developpeur Front` : change la question courante
  - `Developpeur Back` : corrige automatiquement une erreur
  - `Developpeur Mobile` : affiche un indice
- historique des parties
- export PDF d'un rapport de partie
- integration avec MySQL local via XAMPP

## Stack Technique

- `C#`
- `.NET 8`
- `WPF`
- `Entity Framework Core 8`
- `Pomelo.EntityFrameworkCore.MySql`
- `QuestPDF`
- `MahApps.Metro.IconPacks`
- `MySQL / MariaDB (XAMPP)`

## Structure Du Projet

```text
clavierdor/
├── Assets/
├── Data/
├── Migrations/
├── Models/
├── Services/
├── ViewModels/
├── Views/
│   ├── Dialogs/
│   └── Pages/
├── App.xaml
├── MainWindow.xaml
└── clavierdor.csproj
```

### Dossiers principaux

- `Assets`
  - images et ressources visuelles
- `Data`
  - configuration de la base, `DbContext`, initialisation
- `Models`
  - modeles metier : `Player`, `Partie`, `History`, `Question`
- `Services`
  - logique de donnees et generation PDF
- `ViewModels`
  - logique de presentation des pages et dialogs
- `Views`
  - interface utilisateur WPF

## Pages Et Ecrans

### Pages

- `Home`
  - page d'accueil principale
- `Quiz`
  - page de jeu
- `History`
  - historique des parties
- `ExportPDF`
  - selection d'un joueur et export PDF

### Dialogs

- `Play`
  - creation d'une nouvelle partie
- `Resume`
  - reprise d'une partie existante

## Modeles Principaux

### `Player`

Represente un joueur.

Champs principaux :

- `Id`
- `Name`
- `Pouvoir`

### `Partie`

Represente une session de jeu.

Champs principaux :

- `Id`
- `PlayerId`
- `Pouvoir`
- `Category`
- `CurrentQuestionIndex`
- `Score`
- `IsFinished`
- `CreatedAt`
- `FinishedAt`

### `History`

Represente une entree d'historique liee a une partie.

Champs principaux :

- `PartieId`
- `PlayerName`
- `Pouvoir`
- `Category`
- `Score`
- `IsFinished`
- `PlayedAt`
- `WonBoss`
- `BossesKilled`

### `Question`

Represente une question du quiz.

Champs principaux :

- `Category`
- `Text`
- `OptionA`
- `OptionB`
- `OptionC`
- `CorrectAnswer`
- `IsBoss`
- `IsFinalBoss`
- `BossName`

## Base De Donnees

Le projet utilise une base `MySQL/MariaDB` locale.

Connexion par defaut :

```csharp
server=127.0.0.1;port=3306;database=clavierdor_db;user=root;password=;
```

Configuration definie dans :

- [DatabaseSettings.cs](./Data/DatabaseSettings.cs)

### Tables principales

- `players`
- `questions`
- `parties`
- `histories`

## Installation

### Prerequis

- `Visual Studio 2022` ou plus recent avec support `.NET Desktop Development`
- `.NET 8 SDK`
- `XAMPP` avec `MySQL/MariaDB` actif

### Etapes

1. Cloner le projet

```bash
git clone <repo-url>
```

2. Ouvrir la solution

```text
clavierdor.sln
```

3. Demarrer `XAMPP`

- lancer `Apache` si besoin
- lancer `MySQL`

4. Verifier la connexion a la base dans :

- [DatabaseSettings.cs](./Data/DatabaseSettings.cs)

5. Lancer l'application

```bash
dotnet build
dotnet run
```

ou directement depuis Visual Studio.

## Initialisation De La Base

Au demarrage, l'application appelle :

- [App.xaml.cs](./App.xaml.cs)
- [DatabaseInitializer.cs](./Data/DatabaseInitializer.cs)

Ce mecanisme :

- applique les migrations EF Core
- initialise la structure de la base
- ajoute certains correctifs de schema si necessaire

## Remplir La Table `questions`

Les questions sont stockees dans la table `questions`.

Colonnes principales :

- `Category`
- `Text`
- `OptionA`
- `OptionB`
- `OptionC`
- `CorrectAnswer`
- `IsBoss`
- `IsFinalBoss`
- `BossName`

Exemple SQL :

```sql
INSERT INTO questions
(Category, Text, OptionA, OptionB, OptionC, CorrectAnswer, IsBoss, IsFinalBoss, BossName)
VALUES
('anglais', 'What is the plural of mouse?', 'mouses', 'mice', 'mousees', 'B', 0, 0, ''),
('anglais', 'Who guards this level?', 'Boss Anglais', 'Boss SQL', 'Boss Final', 'A', 1, 0, 'Boss Anglais'),
('final', 'Dans mon langage ancien, si 1 + 1 vaut 10, quel est le resultat ?', '2', '10', '0', 'B', 0, 1, 'L Architecte Supreme');
```

## Regles Du Jeu

### Ordre du quiz

Le quiz suit cet ordre :

1. `anglais`
2. `culture_generale`
3. `metiers_informatique`
4. `logique`
5. `algorithmes`

A la fin de chaque categorie :

- une question boss est ajoutee

A la fin du parcours :

- un boss final est ajoute

### Score

- question normale correcte : `+3`
- question normale fausse : `0`
- boss reussi : `+30`
- boss rate : `-10`
- boss final reussi : `+50`
- boss final rate : `-20`

### Evaluation de fin

- score `< 250` : `Defaite`
- score `>= 250` : `Reussi`
- score `> 400` : `Pro`
- score `= 500` : `Legende`

## Export PDF

Le module d'export permet :

- de choisir un joueur
- de recuperer ses donnees d'historique
- de generer un rapport PDF via `QuestPDF`

Fichiers importants :

- [ExportPDF.xaml](./Views/Pages/ExportPDF.xaml)
- [ExportPDF.xaml.cs](./Views/Pages/ExportPDF.xaml.cs)
- [ExportPdfViewModel.cs](./ViewModels/ExportPdfViewModel.cs)
- [PdfExportService.cs](./Services/PdfExportService.cs)
- [ExportReportData.cs](./Services/ExportReportData.cs)

## Architecture Logique

### `GameDataService`

Responsable de :

- creer une partie
- retrouver une partie en cours
- charger l'historique
- charger les joueurs
- recuperer les donnees d'export
- sauvegarder la progression

### `PdfExportService`

Responsable de :

- construire le PDF final a partir des donnees du joueur

### `ExportReportData`

Objet de transport de donnees utilise pour le rapport PDF.

## Lancer Le Projet En Developpement

### Avec Visual Studio

- ouvrir `clavierdor.sln`
- definir `clavierdor` comme projet de demarrage
- lancer avec `F5`

### Avec le terminal

```bash
dotnet build clavierdor.csproj
dotnet run --project clavierdor.csproj
```

## Auteur

Projet realise autour du concept **Clavier D'Or**, un quiz pour developpeurs avec progression, pouvoirs, boss et rapport de partie exportable.
