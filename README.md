BookStore Microservices

A distributed Book Store system built with ASP.NET Core Microservices Architecture.

The project consists of two independent microservices communicating asynchronously through RabbitMQ while maintaining data consistency across service boundaries.

For architecture decisions, implemented patterns, design considerations and technical details please refer to:

docs/CoverLetter.md

Key Features
Catalog Service
Book CRUD Operations
Inventory Management
Redis Caching
Cache Invalidation
Stock Reservation
Ordering Service
Order Creation
Order Tracking
Order Status Management
Event Publishing
Distributed System Features
Event-Driven Architecture
Saga Pattern (Choreography)
Eventual Consistency
Outbox Pattern
Retry Policy (Polly)
Dockerized Deployment
Technology Stack
ASP.NET Core 9
Entity Framework Core
MediatR
SQL Server
RabbitMQ
Redis
Polly
Docker
Docker Compose
Project Structure
BookStore.Catalog.API
BookStore.Catalog.Application
BookStore.Catalog.Domain
BookStore.Catalog.Infrastructure

BookStore.Ordering.API
BookStore.Ordering.Application
BookStore.Ordering.Domain
BookStore.Ordering.Infrastructure

BookStore.Shared.API
BookStore.Shared.Common
Shared.Contracts
Running The Application
Prerequisites
.NET 9 SDK
Docker Desktop
Clone Repository
git clone https://github.com/Hnaeemaei8/BookStoreMicroservices.git
cd BookStoreMicroservices
Start All Services
docker compose up --build

The following containers will be started:

Catalog API
Ordering API
SQL Server
Redis
RabbitMQ
Sample Workflow
Create Order
↓
OrderCreatedEvent
↓
Catalog Service
↓
StockReservedEvent / StockFailedEvent
↓
Ordering Service
↓
Confirmed / Failed