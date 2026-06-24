# CodeLearn

CodeLearn je web aplikacija za učenje C# programskog jezika. Aplikacija omogućava korisnicima da se registruju, prijave, pregledaju kurseve, upisuju kurseve, prate napredak, čitaju lekcije, rešavaju kvizove i dnevne izazove.

Projekat je razvijen kao full-stack aplikacija sa ASP.NET Core Web API backendom, PostgreSQL bazom podataka i React frontend aplikacijom.

## Opis projekta

CodeLearn je platforma namenjena učenju C# programskog jezika kroz strukturisane kurseve.

Korisnik može da:

* registruje nalog
* prijavi se u aplikaciju
* pregleda kurseve
* upiše kurs
* pregleda module i lekcije
* označi lekciju kao završenu
* rešava kvizove
* vidi svoje rezultate
* prati napredak
* rešava dnevni izazov

Administrator može da:

* dodaje, menja i briše kurseve
* dodaje, menja i briše module
* dodaje, menja i briše lekcije
* dodaje, menja i briše kvizove
* dodaje, menja i briše pitanja
* dodaje, menja i briše ponuđene odgovore
* kreira Daily Challenge za određeni datum

Daily Challenge se kreira kroz admin panel, a Hangfire background job automatski aktivira izazov za trenutni datum.



## Tehnologije

Backend

* C#
* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL
* ASP.NET Core Identity
* JWT Authentication
* Repository Pattern
* Unit of Work Pattern
* FluentValidation
* Hangfire
* Swagger / OpenAPI

Frontend

* React
* Vite
* JavaScript
* Axios
* React Router DOM
* CSS

Baza podataka

* PostgreSQL


## Arhitektura projekta

Projekat je organizovan u više slojeva kako bi kod bio pregledniji, modularniji i lakši za održavanje.

CodeLearn
├── CodeLearn.Api
├── CodeLearn.Application
├── CodeLearn.Domain
├── CodeLearn.Infrastructure
├── codelearn-frontend
└── CodeLearn.slnx



### CodeLearn.Domain

CodeLearn.Domain predstavlja domen aplikacije. U ovom projektu se u njemu nalaze osnovni entiteti i enumi.

Ovaj sloj ne zavisi od baze podataka, kontrolera ili korisničkog interfejsa.


Glavni entiteti:

* ApplicationUser
* Course
* CourseModule
* Lesson
* Quiz
* Question
* AnswerOption
* CourseEnrollment
* LessonProgress
* UserQuizResult
* DailyChallenge
* DailyChallengeSubmission

Enumi:

* CourseLevel
* EnrollmentStatus
* QuestionType




### CodeLearn.Application

CodeLearn.Application predstavlja aplikacioni sloj.

U ovom projektu se u njemu nalaze interfejsi za Repository i Unit of Work.

Ovaj sloj definiše šta aplikacija očekuje od infrastrukture, ali ne zna konkretno kako se podaci čuvaju u bazi.

Sadrži:

Common
 └── Interfaces

Glavni interfejsi:

* IGenericRepository<T>
* IUnitOfWork

IGenericRepository<T>

Predstavlja generički repository koji omogućava osnovne operacije nad entitetima:

* GetAllAsync
* GetByIdAsync
* FindAsync
* AddAsync
* Update
* Delete

IUnitOfWork

Objedinjuje sve repository-je i omogućava čuvanje promena kroz jednu metodu:

* SaveChangesAsync

Ovim se postiže bolja organizacija rada sa bazom.



### CodeLearn.Infrastructure

CodeLearn.Infrastructure predstavlja infrastrukturni sloj aplikacije.

U ovom sloju se nalazi konkretna implementacija rada sa bazom podataka.

Sadrži:

Persistence
Repositories

Persistence

U folderu Persistence nalazi se CodeLearnDbContext.

CodeLearnDbContext nasleđuje IdentityDbContext<ApplicationUser>, što znači da aplikacija koristi ASP.NET Identity za korisnike, role, login i sigurnost.

U DbContext su definisani DbSet-ovi za entitete:

* Courses
* CourseModules
* Lessons
* Quizzes
* Questions
* AnswerOptions
* CourseEnrollments
* LessonProgresses
* UserQuizResults
* DailyChallenges
* DailyChallengeSubmissions

Takođe su podešene relacije između entiteta, unique index-i i ponašanje pri brisanju.

Repositories

U folderu Repositories nalaze se:

* GenericRepository<T>
* UnitOfWork

GenericRepository<T> implementira osnovne CRUD operacije pomoću Entity Framework Core-a.

UnitOfWork povezuje sve repository-je i poziva SaveChangesAsync.



### CodeLearn.Api

CodeLearn.Api predstavlja backend API sloj aplikacije.

Ovo je ulazna tačka backend aplikacije. Ovde se nalaze kontroleri, DTO klase, validatori, konfiguracija servisa, autentifikacija, autorizacija i Hangfire job-ovi.

Sadrži:

Controllers
DTOs
Jobs
Validators
Program.cs
appsettings.json

Controllers

Kontroleri izlažu REST API rute koje frontend koristi.

Glavni kontroleri:

* AuthController
* CoursesController
* CourseModulesController
* LessonsController
* QuizzesController
* QuestionsController
* AnswerOptionsController
* CourseEnrollmentsController
* LessonProgressController
* QuizResultsController
* DailyChallengesController



DTOs

DTO klase se koriste za prenos podataka između frontend aplikacije i backend API-ja.

Primeri DTO foldera:

DTOs
├── Auth
├── Courses
├── CourseModules
├── Lessons
├── Quizzes
├── Questions
├── AnswerOptions
├── QuizSubmissions
├── DailyChallenges
└── DailyChallengeSubmissions

DTO klase pomažu da se ne šalju direktno entiteti iz baze, već samo podaci koji su potrebni za određenu operaciju.


Validators

Aplikacija koristi FluentValidation za validaciju ulaznih podataka.

Validatori proveravaju, na primer:

* da naziv kursa nije prazan
* da opis nije prazan
* da su ID vrednosti veće od nule
* da je email validan
* da password ima dovoljnu dužinu
* da je tip pitanja validan enum
* da je passing score između 0 i 100



Jobs

U folderu Jobs nalazi se Hangfire job:

* DailyChallengeJob

Ovaj job proverava koji Daily Challenge ima današnji datum i postavlja ga kao aktivan.

Logika:

1. Uzmi današnji datum
2. Pronađi Daily Challenge za današnji datum
3. Ako postoji, postavi IsActive = true
4. Sačuvaj promene

Hangfire se koristi za automatsko izvršavanje ovog posla u pozadini.



Autentifikacija i autorizacija

Aplikacija koristi JWT autentifikaciju.

Nakon login-a korisnik dobija JWT token. Frontend taj token čuva u localStorage i šalje ga u svakom zaštićenom zahtevu.

Backend koristi:

[Authorize]

za rute kojima može pristupiti svaki prijavljen korisnik.

Za admin funkcionalnosti koristi se:

[Authorize(Roles = "Admin")]

To znači da samo korisnik koji ima rolu Admin može da dodaje, menja i briše sadržaj.

Običan korisnik je ApplicationUser bez admin role.

Admin je ApplicationUser kome je u bazi dodeljena rola Admin.





### codelearn-frontend

codelearn-frontend predstavlja React frontend aplikaciju.

Frontend komunicira sa backendom preko Axios biblioteke.

Struktura frontend projekta

codelearn-frontend
├── public
├── src
│   ├── api
│   ├── components
│   ├── context
│   ├── pages
│   ├── styles
│   ├── App.jsx
│   └── main.jsx
├── package.json
└── vite.config.js

src/api

Sadrži axiosClient.js.

Ovaj fajl podešava osnovni API URL i automatski dodaje JWT token u request header.

src/context

Sadrži AuthContext.jsx.

Ovaj context čuva:

* token
* trenutno prijavljenog korisnika
* login funkciju
* register funkciju
* logout funkciju
* proveru da li je korisnik admin

src/components

Sadrži komponente koje se koriste na više mesta:

* Navbar
* ProtectedRoute
* CourseCard

src/pages

Sadrži glavne stranice aplikacije.

Korisničke stranice:

* HomePage
* LoginPage
* RegisterPage
* CoursesPage
* CourseDetailsPage
* LessonsPage
* QuizPage
* MyProgressPage
* DailyChallengePage

Admin stranice:

* AdminCoursesPage
* AdminModulesPage
* AdminLessonsPage
* AdminQuizzesPage
* AdminQuestionsPage
* AdminAnswerOptionsPage
* AdminDailyChallengesPage

src/styles

Sadrži globalni CSS za izgled aplikacije.

Aplikacija koristi moderan teget-plavi i beli dizajn sa programerskim motivima.



## Funkcionalnosti aplikacije

Korisnik

Korisnik može da:

* se registruje
* se prijavi
* pregleda kurseve
* upiše kurs
* pregleda module
* pregleda lekcije
* označi lekciju kao završenu
* rešava kviz
* vidi rezultate kvizova
* vidi svoj napredak
* rešava Daily Challenge

Admin

Admin može da:

* dodaje kurseve
* menja kurseve
* briše kurseve
* dodaje module
* menja module
* briše module
* dodaje lekcije
* menja lekcije
* briše lekcije
* dodaje kvizove
* menja kvizove
* briše kvizove
* dodaje pitanja
* menja pitanja
* briše pitanja
* dodaje ponuđene odgovore
* menja ponuđene odgovore
* briše ponuđene odgovore
* kreira Daily Challenge


Važni endpoint-i

Auth

POST /api/Auth/register
POST /api/Auth/login
GET  /api/Auth/me

Courses

GET    /api/Courses
GET    /api/Courses/{id}
POST   /api/Courses
PUT    /api/Courses/{id}
DELETE /api/Courses/{id}

CourseModules

GET    /api/CourseModules
GET    /api/CourseModules/{id}
GET    /api/CourseModules/by-course/{courseId}
POST   /api/CourseModules
PUT    /api/CourseModules/{id}
DELETE /api/CourseModules/{id}

Lessons

GET    /api/Lessons
GET    /api/Lessons/{id}
GET    /api/Lessons/by-module/{moduleId}
POST   /api/Lessons
PUT    /api/Lessons/{id}
DELETE /api/Lessons/{id}

Quizzes

GET    /api/Quizzes
GET    /api/Quizzes/{id}
GET    /api/Quizzes/by-lesson/{lessonId}
POST   /api/Quizzes
PUT    /api/Quizzes/{id}
DELETE /api/Quizzes/{id}
POST   /api/Quizzes/{quizId}/submit

Questions

GET    /api/Questions
GET    /api/Questions/{id}
GET    /api/Questions/by-quiz/{quizId}
POST   /api/Questions
PUT    /api/Questions/{id}
DELETE /api/Questions/{id}

AnswerOptions

GET    /api/AnswerOptions
GET    /api/AnswerOptions/{id}
GET    /api/AnswerOptions/by-question/{questionId}
POST   /api/AnswerOptions
PUT    /api/AnswerOptions/{id}
DELETE /api/AnswerOptions/{id}

CourseEnrollments

POST /api/CourseEnrollments/enroll/{courseId}
GET  /api/CourseEnrollments/my-courses
GET  /api/CourseEnrollments/my-courses/{courseId}
PUT  /api/CourseEnrollments/drop/{courseId}

LessonProgress

POST /api/LessonProgress/complete/{lessonId}
GET  /api/LessonProgress/my-progress
GET  /api/LessonProgress/my-progress/by-course/{courseId}

QuizResults

GET /api/QuizResults/my-results
GET /api/QuizResults/my-results/by-quiz/{quizId}
GET /api/QuizResults/my-best-result/{quizId}

DailyChallenges

GET    /api/DailyChallenges
GET    /api/DailyChallenges/{id}
GET    /api/DailyChallenges/today
POST   /api/DailyChallenges
PUT    /api/DailyChallenges/{id}
DELETE /api/DailyChallenges/{id}
POST   /api/DailyChallenges/{dailyChallengeId}/submit
GET    /api/DailyChallenges/my-submissions
GET    /api/DailyChallenges/my-submissions/{dailyChallengeId}


## Sigurnost

* Lozinke se ne čuvaju kao običan tekst.
* ASP.NET Identity čuva hash lozinke.
* JWT token se koristi za autentifikaciju.
* Admin rute su zaštićene pomoću role Admin.



## Autor

Jovana Vuksanović 2022/0293