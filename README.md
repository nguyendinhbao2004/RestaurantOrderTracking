RestaurantOrderTracking (ROT) System

AI-Powered Fulfillment & Management Platform for Modern F&B

📌 Project Overview
RestaurantOrderTracking (ROT) is a next-generation management ecosystem designed to streamline restaurant operations. By leveraging .NET 10 for high-performance processing and Voice AI, ROT allows staff (Chefs, Waiters, and Cashiers) to interact with the system hands-free, minimizing latency in high-pressure environments.

🚀 Key Modules
1. Enterprise Management
Role-Based Access Control (RBAC): Granular permissions for Admins, Managers, Waiters, Chefs, and Cashiers.

Inventory & Menu Orchestration: Real-time synchronization of dish availability and pricing.

Analytical Dashboards: Comprehensive reporting powered by PostgreSQL's advanced indexing.

2. Voice AI Integration
The system implements a specialized pipeline to transform vocal commands into system events:

Natural Language Ordering: "Add two Wagyu Burgers and one Red Wine to Table 10."

Workflow Automation: Chefs can confirm preparation status via voice, instantly notifying the service staff.

Intent Recognition: Advanced NLP parsing to extract actions and entities from raw audio streams.

🏗 System Architecture
The platform is built on a Microservices Architecture to ensure independent scalability and fault tolerance.

Data Flow Diagram (DFD) Analysis
Input Layer: Handles RESTful requests, WebSocket streams, and Audio buffers.

Intelligence Layer: * Speech-to-Text (STT): High-fidelity audio transcription.

NLP Service: Intent classification and entity extraction.

Persistence Layer: Utilizes PostgreSQL 18 for relational data integrity and complex querying capabilities.

🛠 Tech Stack
Backend: ASP.NET Core 10 (Web API)

Database: PostgreSQL 18 (Relational Storage)

AI/NLP: Google Cloud Speech-to-Text / Custom NLP Engine

Containerization: Docker
