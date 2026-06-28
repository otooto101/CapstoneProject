const fs = require("fs");
const {
  Document, Packer, Paragraph, TextRun, Table, TableRow, TableCell,
  Footer, AlignmentType, LevelFormat, HeadingLevel, TableOfContents,
  BorderStyle, WidthType, ShadingType, PageNumber, NumberFormat
} = require("docx");

// ---- measurements (A4, 1cm = 567 DXA) ----
const PAGE = {
  size: { width: 11906, height: 16838 },
  margin: { top: 1417, right: 850, bottom: 1417, left: 1701 } // L 3cm, R 1.5cm, T/B 2.5cm
};
const CONTENT_W = 11906 - 1701 - 850; // 9355
const GRAY = "6A6A6A";
const HDR_FILL = "DDE6F0";
const BORD = { style: BorderStyle.SINGLE, size: 4, color: "BBBBBB" };
const BORDERS = { top: BORD, bottom: BORD, left: BORD, right: BORD };

// ---- helpers ----
const run = (text, o = {}) => new TextRun({ text, ...o });
const para = (text, o = {}) => new Paragraph({ children: [run(text)], ...o });
const h1 = (text, brk = true) => new Paragraph({ heading: HeadingLevel.HEADING_1, pageBreakBefore: brk, children: [run(text)] });
const h2 = (text) => new Paragraph({ heading: HeadingLevel.HEADING_2, children: [run(text)] });
const h3 = (text) => new Paragraph({ heading: HeadingLevel.HEADING_3, children: [run(text)] });
const bullet = (text) => new Paragraph({ numbering: { reference: "bul", level: 0 }, children: [run(text)] });
const numItem = (text) => new Paragraph({ numbering: { reference: "num", level: 0 }, children: [run(text)] });
const noteLabel = () => new Paragraph({ spacing: { before: 80, after: 20 }, children: [run("To write here:", { italics: true, color: GRAY, bold: true })] });
const noteItem = (text) => new Paragraph({ numbering: { reference: "note", level: 0 }, children: [run(text, { italics: true, color: GRAY })] });
const notes = (arr) => [noteLabel(), ...arr.map(noteItem)];
const refItem = (text) => new Paragraph({ spacing: { after: 80 }, indent: { left: 480, hanging: 480 }, children: [run(text)] });
const titleLine = (text, o = {}) => new Paragraph({
  alignment: AlignmentType.CENTER,
  spacing: { line: 240, after: 0, before: o.before || 0 },
  children: [run(text, { size: o.size || 24, bold: !!o.bold, italics: !!o.italics, font: o.font || "Times New Roman" })]
});

// ---- contributions table ----
const COLS = [1850, 2350, 5155];
function contribCell(text, opts) {
  return new TableCell({
    borders: BORDERS,
    width: { size: opts.w, type: WidthType.DXA },
    shading: opts.head ? { fill: HDR_FILL, type: ShadingType.CLEAR, color: "auto" } : undefined,
    margins: { top: 80, bottom: 80, left: 120, right: 120 },
    children: [new Paragraph({ children: [run(text, { bold: !!opts.head })] })]
  });
}
function contribRow(a, b, c, head) {
  return new TableRow({
    tableHeader: !!head,
    children: [contribCell(a, { w: COLS[0], head }), contribCell(b, { w: COLS[1], head }), contribCell(c, { w: COLS[2], head })]
  });
}
const contributionsTable = new Table({
  width: { size: CONTENT_W, type: WidthType.DXA },
  columnWidths: COLS,
  rows: [
    contribRow("Member", "Area", "Main work in the project", true),
    contribRow("Nika Edisherashvili", "Frontend and user interface",
      "Built the whole web front with ASP.NET Core MVC and Razor views. Designed the visual style and the page layout. Added the dark and light theme switch and the brain mascot. Built the swipe based discover screen and the look of the dashboard and the analyze pages."),
    contribRow("Oto Katsadze", "Architecture, domain, data and API",
      "Set up the clean architecture and the project structure. Built the domain entities and the digital twin model. Built the persistence layer with EF Core 10 repositories and the database context. Built the auth base with Identity, JWT and user secrets. Built the API layer with Swagger."),
    contribRow("Beka Baratashvili", "Application logic and AI",
      "Built the application layer with MediatR commands and queries. Built the infrastructure layer and the AI part. Connected Semantic Kernel with Google Gemini for chat and embeddings. Built the RAG workflow with embedding generation and cosine similarity. Built the decision analysis and the option scoring.")
  ]
});

// ---- Section 1: title page ----
const titlePage = [
  titleLine("სსიპ - ქუთაისის საერთაშორისო უნივერსიტეტი", { size: 28, bold: true, font: "Sylfaen", before: 700 }),
  titleLine("Kutaisi International University", { size: 28, bold: true, font: "Sylfaen", before: 120 }),
  titleLine("School of Computer Science", { size: 28, bold: true, font: "Sylfaen", before: 120 }),
  titleLine("Bachelor's Educational Program in Computer Science", { size: 28, bold: true, font: "Sylfaen", before: 120 }),
  titleLine("Nika Edisherashvili", { size: 32, bold: true, font: "Sylfaen", before: 1000 }),
  titleLine("Oto Katsadze", { size: 32, bold: true, font: "Sylfaen", before: 150 }),
  titleLine("Beka Baratashvili", { size: 32, bold: true, font: "Sylfaen", before: 150 }),
  titleLine("DigitalTwin: Personal AI Navigator", { size: 32, bold: true, font: "Sylfaen", before: 950 }),
  titleLine("A private AI life advisor that uses a digital twin and long term memory to guide real decisions", { size: 24, font: "Sylfaen", before: 160 }),
  titleLine("Submitted in partial fulfillment of the requirements for the degree of", { size: 28, font: "Sylfaen", before: 950 }),
  titleLine("Bachelor of Science in Computer Science", { size: 28, font: "Sylfaen", before: 120 }),
  titleLine("Thesis Supervisor: Ana Idadze", { size: 24, font: "Sylfaen", before: 950 }),
  titleLine("Professor, Kutaisi International University", { size: 24, font: "Sylfaen", before: 150 }),
  titleLine("Kutaisi, 2026", { size: 24, font: "Sylfaen", before: 1100 })
];

// ---- Section 2: front matter ----
const declaration = [
  h1("Declaration", false),
  para("We, the authors of this bachelor's thesis, declare that the thesis is our own original work and does not contain any material previously published, submitted for publication, or presented for defense by other authors, unless properly referenced or cited in accordance with academic standards.", { spacing: { after: 240 } }),
  para("Authors: Nika Edisherashvili, Oto Katsadze, Beka Baratashvili", { spacing: { after: 120 } }),
  para("Signatures: ______________________     ______________________     ______________________", { spacing: { after: 120 } }),
  para("Place: Kutaisi          Date: ____ / ____ / 2026")
];
const abstract = [
  h1("Abstract", true),
  para("Some choices barely register. Others can tip your whole life sideways. Dropping out of a degree. Walking away from a relationship you built your twenties around. People tend to hit those moments alone, late at night, with a hundred thoughts and no order to them. That is the moment we built LifeAdvisor for."),
  para("It is a web app that gets to know you, slowly, then helps you think one hard decision through in private. You type out what is eating at you. It reads that, pulls up what it already remembers about you, and comes back with three honest options. Each gets a score out of a hundred and one next move to try. You pick the one that feels right, and it files the choice away for next time."),
  para("Behind it sits .NET and C#, a Google Gemini model wired in through Microsoft Semantic Kernel, and some cosine similarity that digs up your past decisions. It is young and rough. But even now, the memory is the part that makes the advice feel like it knows you."),
  new Paragraph({ spacing: { before: 160 }, children: [run("Keywords: ", { bold: true }), run("digital twin, decision support, artificial intelligence, retrieval augmented generation, text embeddings, .NET, Semantic Kernel, life decisions.")] })
];
const contributions = [
  h1("Author Contributions", true),
  para("This thesis and the project behind it were made by three students. The table below shows who worked on each part. It can be edited later if the split changes.", { spacing: { after: 160 } }),
  contributionsTable,
  para("All three members worked together on planning, testing, code reviews and joining the parts into one working app.", { spacing: { before: 160 } })
];
const toc = [
  new Paragraph({ pageBreakBefore: true, alignment: AlignmentType.CENTER, spacing: { after: 80 }, children: [run("Table of Contents", { bold: true, size: 30 })] }),
  new Paragraph({ alignment: AlignmentType.CENTER, spacing: { after: 160 }, children: [run("After you open the file in Word, right click on the table below and choose Update Field to fill in the page numbers.", { italics: true, color: GRAY, size: 20 })] }),
  new TableOfContents("Table of Contents", { hyperlink: true, headingStyleRange: "1-3" })
];
const frontMatter = [...declaration, ...abstract, ...contributions, ...toc];

// ---- Section 3: body ----
const intro = [
  h1("1. Introduction"),
  para("People make decisions every day. Most of them are small. But some are big and they can change a whole life. Things like changing a job, moving to a new city, ending a relationship or going back to study. These choices are hard because they mix feelings, money, time and other people. When there is too much to think about at once a person can feel stuck. This thesis is about a web app that tries to help in that moment. The app is called LifeAdvisor and the working title of the project is DigitalTwin: Personal AI Navigator."),
  h2("1.1 Background and Motivation"),
  para("Today many people use simple chatbots to ask for advice. The problem is that a normal chatbot forgets the user after the chat ends. It does not really know who the person is. Our idea is different. We build a digital twin of the user. The twin keeps private notes about the user and learns from past choices. So the advice fits the real person and not a random one."),
  ...notes(["Add a few real numbers about stress and decision fatigue with IEEE citations.", "Fun fact to weave in: a widely cited 2011 study of court decisions found that judges ruled more favorably early in the day and right after breaks. It is debated, but it is a memorable way to open the idea of decision fatigue.", "Explain in one short paragraph why a normal chatbot is not enough for personal choices.", "Say why privacy matters for this kind of personal data."]),
  h2("1.2 Problem Statement"),
  para("The main problem is simple to say. People need help to think through hard personal choices, but the tools they have do not remember them and do not show clear options. We want to solve this with one private app that has memory and gives scored options."),
  h2("1.3 Aim and Objectives"),
  para("The aim of the project is to build a working web app that gives personal and private decision support with the help of AI and a digital twin model. The objectives are listed below."),
  numItem("Design a clean and layered software structure that is easy to grow."),
  numItem("Build a digital twin that stores the user story in a private way."),
  numItem("Connect an AI model that can chat and make text embeddings."),
  numItem("Build a retrieval step so the AI can use past decisions of the user."),
  numItem("Give the user clear guidance and three scored options for each decision."),
  numItem("Save every decision in a personal history."),
  h2("1.4 Scope of the Project"),
  para("This is a capstone project so it is a first version and not a finished product. The main flow of register, onboarding, analysis and history works. Some parts like the tests and a few screens still need more work. The app runs on the web for now. A mobile app can come later."),
  ...notes(["List clearly what is in scope and what is out of scope.", "Mention that this version runs on localhost with SQL Server Express."]),
  h2("1.5 Structure of the Thesis"),
  para("The thesis has four main chapters. Chapter one is this introduction. Chapter two looks at other work and ideas that are close to ours. Chapter three explains how we built the app and shows the results. Chapter four gives the conclusion and the next steps.")
];

const lit = [
  h1("2. Literature Review"),
  para("This chapter looks at the main ideas and tools behind the project. It connects them to our problem of personal decision support. The references use the IEEE style with numbers in square brackets."),
  h2("2.1 Decision Support Systems"),
  para("A decision support system is software that helps a person choose between options. The idea is old but it fits our work. Our app is a modern decision support system for personal life choices [5]."),
  ...notes(["Describe what a decision support system is and give one or two classic examples [5].", "Say how a personal life advisor is similar and how it is different."]),
  h2("2.2 Digital Twins"),
  para("A digital twin is a software copy of a real thing. It is used a lot in industry for machines and factories [4]. In our project the real thing is a person. The twin holds the user story and helps the AI understand the user."),
  ...notes(["Explain the origin of the digital twin idea [4].", "Show how we move the idea from machines to a person.", "Fun fact to weave in: the digital twin idea is often traced to NASA, which kept full mockups and simulators of its spacecraft. During Apollo 13 the ground team used these copies to test fixes before the crew tried them. Today Formula 1 teams run a digital twin of the car during a race."]),
  h2("2.3 Large Language Models and Text Embeddings"),
  para("Large language models can read and write text in a human way. They are built on the transformer design [3]. A text embedding turns a piece of text into a list of numbers. Texts with a close meaning get close numbers [12], [13]. We use embeddings to compare decisions by meaning and not just by words."),
  ...notes(["Give a short and simple explanation of transformers and embeddings [3], [13], [14].", "Explain why meaning based search is better than keyword search here.", "Fun fact to weave in: the classic example of embeddings is that king minus man plus woman lands close to queen."]),
  h2("2.4 Retrieval Augmented Generation"),
  para("Retrieval augmented generation, or RAG, is a method that gives extra context to a language model before it answers [2]. First the system finds text that is related to the question. Then it puts that text into the prompt. This makes the answer more correct and more personal. Our app uses this method with the past decisions of the user."),
  ...notes(["Explain the RAG idea in two or three simple sentences [2].", "Say why RAG is a good fit for giving the AI a memory of the user."]),
  h2("2.5 AI Coaching and Wellbeing Apps"),
  para("There are apps that try to coach people or help with mood and habits. They show that people are open to talk to software about personal things."),
  ...notes(["Add two or three real example apps and short notes about them with citations.", "Point out what they miss that our app tries to add, like long term memory and scored options."]),
  h2("2.6 Summary and Gap"),
  para("The tools and ideas above each solve a part of the problem. But we did not find one simple app that joins a private digital twin, a memory based retrieval step and clear scored options for personal life choices. This gap is what our project tries to fill.")
];

const method = [
  h1("3. Methodology and Results"),
  para("This chapter is the core of the thesis. It explains how the app was built and what it can do. It covers the way we worked, the design of the system, the backend, the frontend, the security and the AI. At the end it shows the results and the limits."),
  h2("3.1 Development Approach"),
  para("We worked as a team of three. We used Git and GitHub for the code and for branches. Each member worked on a feature branch and we joined the work with pull requests. We split the project by layers so we could work at the same time without too many clashes. We used Visual Studio and .NET 10 for the build."),
  ...notes(["Add a short note on the team workflow and how often you met.", "Mention the branch and pull request style with one example from the repo."]),
  h2("3.2 Requirements and Main User Flow"),
  para("The main flow has a few clear steps. The user registers an account. Then the user goes through onboarding and answers a few questions so the twin can learn the basics. After that the user can open the analyze screen and write a decision. The app returns guidance and three scored options. The user picks one option and the decision is saved in the history. There is also a discover screen with a swipe based way to pick interests."),
  ...notes(["List the functional requirements as short points.", "List the non functional requirements like privacy, speed and ease of use.", "Add a simple flow diagram of register, onboarding, analyze and history and put it in the appendix."]),
  h2("3.3 System Architecture"),
  para("The project follows Clean Architecture. The code is split into layers so each layer has one clear job. The inner layers do not depend on the outer ones. This keeps the rules of the app away from the tools and frameworks. The solution has six projects."),
  bullet("Domain holds the core entities and the digital twin model. It has no dependencies."),
  bullet("Application holds the business logic. It uses MediatR with commands and queries in a CQRS style."),
  bullet("Infrastructure holds the outside services like the AI and the JWT token maker."),
  bullet("Persistence holds the EF Core repositories, the database context and the migrations."),
  bullet("API is the web API with Swagger for testing the endpoints."),
  bullet("Web is the MVC frontend with Razor views."),
  ...notes(["Add a layered architecture diagram and refer to it here [1].", "Explain in one paragraph why clean architecture was a good choice for a team project."]),
  h2("3.4 Backend Design"),
  para("The backend is written in C# on .NET 10. It is the engine of the app. The next parts explain each backend layer."),
  h3("3.4.1 Domain Layer and the Digital Twin Model"),
  para("The domain layer holds the main entities. The center of it is the DigitalTwin. A twin has a preferred name, a date of birth, a location and a life stage. A twin also holds a list of narratives. A narrative is a piece of the user story. There are eight narrative types, for example defining moment, hardest decision, values, life vision and current struggles. A narrative can also be a saved decision with its options and the chosen path. Each narrative can hold a text embedding so it can be compared by meaning."),
  ...notes(["Add a class diagram of the domain entities in the appendix and refer to it.", "List the eight narrative types and say in one line why each one is useful for advice."]),
  h3("3.4.2 Application Layer with MediatR"),
  para("The application layer holds the use cases. It uses the MediatR library and a CQRS style. A command changes data, for example register user, complete onboarding, analyze decision and complete decision. A query reads data, for example get decision history, get onboarding status and get the swipe deck. Each command and query has its own handler. This keeps the code small and easy to test."),
  ...notes(["Pick one command and walk through its handler step by step as an example.", "Explain why CQRS keeps the code clean for a team."]),
  h3("3.4.3 Persistence Layer with EF Core"),
  para("The persistence layer uses Entity Framework Core 10. It talks to SQL Server Express on localhost during development. It uses the repository pattern and a unit of work. Migrations build and update the database tables for life stages, narratives, analysis settings, decision history and interests."),
  ...notes(["List the main tables and what they store.", "Show one migration as an example of how the schema changed over time."]),
  h3("3.4.4 API Layer with Swagger"),
  para("The API project is an ASP.NET Core web API. It exposes endpoints, for example for auth. It uses Swagger with Swashbuckle so the team can test the endpoints in the browser without extra tools. The API uses JWT tokens to protect the routes."),
  ...notes(["Add a screenshot of the Swagger page in the appendix.", "List the main endpoints in a small table."]),
  h2("3.5 Frontend Design"),
  para("The frontend is an ASP.NET Core MVC app with Razor views. It does not use Blazor so there is no mix up. The design is clean and calm because the topic is personal. The frontend has a dark and light theme and the user choice is saved in the browser. There is a small brain mascot called BrainBuddy that gives the app a friendly face. The discover screen uses a swipe style like a card deck so the user can pick interests in a fun way. Forms show a spinner on submit so the page never feels frozen. This whole part was designed and built by Nika."),
  ...notes(["Add screenshots of the main screens like login, onboarding, analyze, history and discover.", "Explain the design choices for colors, fonts and layout.", "Say how the pages stay usable on small screens."]),
  h2("3.6 Authentication and Security"),
  para("Security is important because the data is personal. The app uses ASP.NET Core Identity for register and login. The web app uses sessions to track the signed in user. The API uses JWT bearer tokens. Secret keys, like the Gemini API key, are kept in user secrets so they are not pushed to GitHub by mistake."),
  ...notes(["Explain the difference between the session based web and the token based API.", "Note any other security steps like input checks and anti forgery tokens in forms."]),
  h2("3.7 The AI Engine: Semantic Kernel and Gemini"),
  para("The AI part is built with Microsoft Semantic Kernel. Semantic Kernel is a toolkit that makes it easier to work with AI models in .NET. We use the Google Gemini connector for two jobs. The first job is chat, which writes the guidance and the options. The second job is embeddings, which turns text into numbers for the retrieval step. A factory class builds the kernel from the settings so the model name and the key stay in one place."),
  ...notes(["Name the exact Gemini chat model and embedding model you used.", "Explain in simple words what Semantic Kernel adds on top of a raw API call."]),
  h2("3.8 The Retrieval Augmented Generation Pipeline"),
  para("The RAG pipeline gives the AI a memory of the user. The steps are shown below."),
  numItem("The user writes a new decision."),
  numItem("The app makes an embedding of that text with Gemini."),
  numItem("The app reads the past narratives of the user and compares them with cosine similarity."),
  numItem("It keeps only the ones above a similarity threshold and takes the top few."),
  numItem("It puts those related decisions into the prompt as extra context."),
  numItem("The AI then writes advice that fits the real history of the user."),
  para("The similarity threshold and the number of related decisions can be changed in the settings. The default threshold is 0.75 and the default number is five. The cosine similarity is computed in C# so the app does not need a special database type."),
  ...notes(["Add the cosine similarity formula and a short and simple explanation.", "Explain what the threshold does and how a higher or lower value changes the result.", "Fun fact to weave in: cosine similarity is the same basic idea that streaming apps use to find songs or films close to your taste. Here it finds past decisions close to the new one."]),
  h2("3.9 Decision Analysis and Option Scoring"),
  para("When the user asks for help the app does not give one flat answer. The guidance is split into clear sections like the situation, what seems to drive it, the risks to watch and a grounded next step. After the guidance the app gives exactly three options. Each option has a title, a score out of one hundred, a short summary, the best case to choose it, the main risk and a first step. The user reads the three options and picks one. That choice marks the decision as completed and saves it to the history."),
  ...notes(["Show one full real example of the guidance and the three options in the appendix.", "Explain how the score is read from the model output and kept between zero and one hundred.", "Note the fallback options that are used if the model output cannot be read."]),
  h2("3.10 Data Storage and Migrations"),
  para("All data is stored in SQL Server. The narratives table holds the user story and the decisions. The text embedding is stored as a JSON array in a text column. We chose this so the app works on SQL Server 2022 and not only on the newest version. The cosine similarity runs in C# over these arrays. EF Core migrations build the tables and keep the schema in sync with the code."),
  ...notes(["Add the database diagram in the appendix and refer to it.", "Mention the trade off of storing embeddings as JSON versus a native vector type."]),
  h2("3.11 Testing"),
  para("The solution has two test projects that use xUnit. One is for unit tests and one is for integration tests. The tests check parts of the application and the data layer. This area is still a work in progress and we plan to add more tests."),
  ...notes(["List the tests you already have and what they cover.", "Add a short plan for the tests you still want to write."]),
  h2("3.12 Results"),
  para("The first version of the app works for the main flow. A user can register, finish onboarding, write a decision and get guidance with three scored options, then pick one and see it in the history. The discover screen and the settings also work. The parts that still need work are some of the tests and a few smaller screens."),
  ...notes(["Add screenshots that show a full run from start to finish.", "If you have any feedback from test users, add it here.", "Be honest about bugs and limits that you found while testing."])
];

const conclusion = [
  h1("4. Conclusion"),
  h2("4.1 Summary of Work"),
  para("In this project we built a web app that helps people think through hard life choices. We used a clean and layered design with .NET and C#. We built a digital twin that keeps a private user story. We connected Semantic Kernel and Google Gemini for chat and embeddings. We added a retrieval step so the advice fits the real user. The main flow works as a first version."),
  h2("4.2 Significance"),
  para("The project shows that a digital twin idea can support real and personal decision making. It joins memory, retrieval and scored options in one simple app. This is the part that we did not find ready made in other tools."),
  h2("4.3 Limitations"),
  para("The app is a first version. It runs on localhost and the tests are not complete. The advice depends on the AI model so the quality can change. The cosine similarity runs in C# which is fine for now but may get slow with a very large history."),
  h2("4.4 Future Work"),
  ...notes(["List the next features you want, for example a mobile app and a real scenario simulation.", "Mention moving to a native vector search if the data grows.", "Mention more tests and a real user study."])
];

const references = [
  h1("Bibliography"),
  para("The references use the IEEE style. Update the details and add the access dates before the final hand in.", { spacing: { after: 160 } }),
  refItem("[1] R. C. Martin, Clean Architecture: A Craftsman Guide to Software Structure and Design. Boston, MA, USA: Prentice Hall, 2017."),
  refItem("[2] P. Lewis et al., Retrieval augmented generation for knowledge intensive NLP tasks, in Proc. Advances in Neural Information Processing Systems (NeurIPS), 2020, pp. 9459 to 9474."),
  refItem("[3] A. Vaswani et al., Attention is all you need, in Proc. Advances in Neural Information Processing Systems (NeurIPS), 2017, pp. 5998 to 6008."),
  refItem("[4] M. Grieves and J. Vickers, Digital twin: mitigating unpredictable, undesirable emergent behavior in complex systems, in Transdisciplinary Perspectives on Complex Systems. Cham, Switzerland: Springer, 2017, pp. 85 to 113."),
  refItem("[5] D. J. Power, Decision Support Systems: Concepts and Resources for Managers. Westport, CT, USA: Quorum Books, 2002."),
  refItem("[6] Microsoft, Semantic Kernel documentation, Microsoft Learn, 2024. [Online]. Available: https://learn.microsoft.com/semantic-kernel/"),
  refItem("[7] Microsoft, .NET documentation, Microsoft Learn, 2025. [Online]. Available: https://learn.microsoft.com/dotnet/"),
  refItem("[8] Microsoft, Entity Framework Core documentation, Microsoft Learn, 2025. [Online]. Available: https://learn.microsoft.com/ef/core/"),
  refItem("[9] Microsoft, ASP.NET Core MVC overview, Microsoft Learn, 2025. [Online]. Available: https://learn.microsoft.com/aspnet/core/mvc/"),
  refItem("[10] Google, Gemini API documentation, Google AI for Developers, 2025. [Online]. Available: https://ai.google.dev/"),
  refItem("[11] M. Jones, J. Bradley, and N. Sakimura, JSON Web Token (JWT), RFC 7519, Internet Engineering Task Force, May 2015. [Online]. Available: https://www.rfc-editor.org/rfc/rfc7519"),
  refItem("[12] D. Jurafsky and J. H. Martin, Speech and Language Processing, 3rd ed. draft, 2023. [Online]. Available: https://web.stanford.edu/jurafsky/slp3/"),
  refItem("[13] T. Mikolov, K. Chen, G. Corrado, and J. Dean, Efficient estimation of word representations in vector space, in Proc. Int. Conf. Learning Representations (ICLR) Workshop, 2013."),
  refItem("[14] J. Devlin, M. Chang, K. Lee, and K. Toutanova, BERT: pre training of deep bidirectional transformers for language understanding, in Proc. NAACL HLT, 2019, pp. 4171 to 4186.")
];

const appendices = [
  h1("Appendices"),
  para("This part holds extra material that is too big or too detailed for the main text.", { spacing: { after: 120 } }),
  h2("Appendix A. Repository and Project Structure"),
  ...notes(["Add the GitHub link of the project.", "Add a screenshot of the solution with the six projects."]),
  h2("Appendix B. Screenshots of the App"),
  ...notes(["Add screenshots of login, onboarding, analyze, the three options, history and discover."]),
  h2("Appendix C. Database Diagram"),
  ...notes(["Add the entity relationship diagram of the database."]),
  h2("Appendix D. Example AI Output"),
  ...notes(["Paste one real run with the full guidance and the three scored options."]),
  h2("Appendix E. Class and Flow Diagrams"),
  ...notes(["Add the class diagram of the domain and the flow diagram of the RAG pipeline."])
];

const body = [...intro, ...lit, ...method, ...conclusion, ...references, ...appendices];

const pageFooter = () => new Footer({ children: [new Paragraph({ alignment: AlignmentType.CENTER, children: [new TextRun({ children: [PageNumber.CURRENT] })] })] });

const doc = new Document({
  creator: "Nika, Oto, Beka",
  title: "DigitalTwin: Personal AI Navigator",
  styles: {
    default: { document: { run: { font: "Times New Roman", size: 24 }, paragraph: { spacing: { line: 360, after: 120 }, alignment: AlignmentType.JUSTIFIED } } },
    paragraphStyles: [
      { id: "Heading1", name: "Heading 1", basedOn: "Normal", next: "Normal", quickFormat: true, run: { font: "Times New Roman", size: 30, bold: true, color: "000000" }, paragraph: { spacing: { before: 240, after: 160 }, outlineLevel: 0, keepNext: true, alignment: AlignmentType.LEFT } },
      { id: "Heading2", name: "Heading 2", basedOn: "Normal", next: "Normal", quickFormat: true, run: { font: "Times New Roman", size: 26, bold: true, color: "000000" }, paragraph: { spacing: { before: 200, after: 120 }, outlineLevel: 1, keepNext: true, alignment: AlignmentType.LEFT } },
      { id: "Heading3", name: "Heading 3", basedOn: "Normal", next: "Normal", quickFormat: true, run: { font: "Times New Roman", size: 24, bold: true, color: "000000" }, paragraph: { spacing: { before: 160, after: 80 }, outlineLevel: 2, keepNext: true, alignment: AlignmentType.LEFT } }
    ]
  },
  numbering: {
    config: [
      { reference: "bul", levels: [{ level: 0, format: LevelFormat.BULLET, text: "•", alignment: AlignmentType.LEFT, style: { paragraph: { indent: { left: 720, hanging: 360 } } } }] },
      { reference: "note", levels: [{ level: 0, format: LevelFormat.BULLET, text: "–", alignment: AlignmentType.LEFT, style: { paragraph: { indent: { left: 720, hanging: 360 } } } }] },
      { reference: "num", levels: [{ level: 0, format: LevelFormat.DECIMAL, text: "%1.", alignment: AlignmentType.LEFT, style: { paragraph: { indent: { left: 720, hanging: 360 } } } }] }
    ]
  },
  sections: [
    { properties: { page: { ...PAGE, pageNumbers: { start: 1, formatType: NumberFormat.LOWER_ROMAN } } }, children: titlePage },
    { properties: { page: { ...PAGE, pageNumbers: { formatType: NumberFormat.LOWER_ROMAN } } }, footers: { default: pageFooter() }, children: frontMatter },
    { properties: { page: { ...PAGE, pageNumbers: { start: 1, formatType: NumberFormat.DECIMAL } } }, footers: { default: pageFooter() }, children: body }
  ]
});

Packer.toBuffer(doc).then((buf) => { fs.writeFileSync("DigitalTwin_Thesis.docx", buf); console.log("written bytes:", buf.length); });
