BookStore Microservices

A distributed Book Store system built with ASP.NET Core Microservices Architecture.

This project demonstrates implementation of modern distributed systems concepts such as Saga Pattern, Eventual Consistency, Outbox Pattern, Redis Caching, RabbitMQ Messaging and Dockerized deployment.

Key Features
Catalog Service
Book CRUD Operations
Inventory Management
Redis Cache Aside Pattern
Cache Invalidation
Stock Reservation
Ordering Service
Order Creation
Order Tracking
Order Status Management
Outbox Pattern
Event Publishing
Distributed System Features
Event-Driven Architecture
Saga Pattern (Choreography)
Eventual Consistency
Durable Messaging
Retry Policy (Polly)
Dockerized Deployment
Architecture & Patterns

The project follows:

Clean Architecture
CQRS
MediatR
Repository Pattern
Unit Of Work Pattern
Event-Driven Architecture
Saga Pattern (Choreography)
Eventual Consistency
Outbox Pattern
Cache Aside Pattern
Retry Pattern (Polly)
Technology Stack
Backend
ASP.NET Core 9
Entity Framework Core
MediatR
Database
SQL Server
Messaging
RabbitMQ
Caching
Redis
Resiliency
Polly
Containerization
Docker
Docker Compose
Solution Structure
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
Saga Workflow
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
Running The Application
Prerequisites
.NET 9 SDK
Docker Desktop
SQL Server (if running without Docker)
Start All Services
docker compose up --build

The following containers will be started:

Catalog API
Ordering API
SQL Server
Redis
RabbitMQ
Sample Scenarios
Successful Order
Create Book (Stock = 10)
Create Order (Quantity = 2)
Order Status → Confirmed
Remaining Stock → 8
Failed Order
Create Book (Stock = 1)
Create Order (Quantity = 5)
Order Status → Failed
Stock Remains Unchanged
Future Improvements
API Gateway
OpenTelemetry
Distributed Tracing
Health Checks
Dead Letter Queues
Centralized Logging
Integration Tests
Authentication & Authorization