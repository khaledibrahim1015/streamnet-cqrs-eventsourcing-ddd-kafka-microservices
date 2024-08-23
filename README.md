## streamnet-cqrs-eventsourcing-ddd-kafka-microservices

# CQRS &  Event Sourcing and Transactional OutBox Pattern with .NET Microservices

## Overview

This project demonstrates a robust implementation of CQRS (Command Query Responsibility Segregation) and Event Sourcing patterns in .NET microservices. It's built without relying on external CQRS frameworks or Mediator frameworks like MediatR , showcasing how to handle commands, raise events, and structure microservices using Domain-Driven Design (DDD) best practices.

![architecture](https://github.com/user-attachments/assets/1883fce9-0113-4b90-bf09-a87c2e8a67a4)

## Key Features

- **Command and Event Handling**: Structured approach to managing commands and events within a CQRS framework.
- **Mediator Pattern**: Implementation of command and query dispatchers using the mediator pattern.
- **Aggregate State Management**: Creation and modification of aggregate states using event messages.
- **Event Store**: MongoDB-based event store serving as the write database.
- **Read Database**: Microsoft SQL Server optimized for query operations.
- **Event Versioning**: Techniques for versioning events to ensure backward compatibility.
- **Optimistic Concurrency Control**: Strategies for handling concurrent modifications in a distributed environment.
- **Message Bus**: Apache Kafka integration for reliable inter-service communication.
- **Transactional Outbox Pattern**: Ensuring consistency between database operations and message publishing.
- **Event Store Replay**: Capabilities to recreate aggregate states and read databases.
- **Database-Per-Service Pattern**: Independent database management for each microservice.
- **Entity Framework Core Integration**: Database interactions managed through EF Core.
- **Dependency Injection**: Effective management of service lifetimes and dependencies.
- **Docker Containerization**: Microservices containerized for consistent environments and simplified deployments.

## Architecture

The project follows a microservices architecture with CQRS and Event Sourcing at its core. Here's an overview of the key components:

### Command and Event Flow

1. Commands are dispatched through a mediator.
2. Command handlers process commands and generate events.
3. Events are stored in the event store (MongoDB).
4. Event handlers update the read database (MS SQL Server).
5. Queries are executed against the read database.
![Mediator-Command-Dispatching](https://github.com/user-attachments/assets/77245768-08cf-4b43-814b-dd7e85e375b9)
![Mediator-Query-Dispatching](https://github.com/user-attachments/assets/264bf377-dff7-44fa-baf7-f0b295c585eb)

### Message Bus

Apache Kafka serves as the message bus for inter-service communication:

![kafkaArchitecture](https://github.com/user-attachments/assets/6390c35b-80bf-4cb2-9e0a-ba596586bfcb)
![apachekafkaConsumer](https://github.com/user-attachments/assets/bf4af436-9e29-4b4e-b628-4fafba1f7bad)

### Transactional Outbox Pattern

To ensure consistency between database operations and message publishing, we've implemented the Transactional Outbox Pattern:

![TRANSACTIONALOUTBOXPATTERN](https://github.com/user-attachments/assets/0d11e493-f52c-45fc-8801-b237f5ab86c8)

1. Database operations and message insertions into an outbox table occur in a single transaction.
2. A separate process periodically checks the outbox table and publishes messages to Kafka.
3. Successfully published messages are marked as processed in the outbox table.

## Key Concepts

1. **CQRS (Command Query Responsibility Segregation)**: Separates read and write operations for improved scalability and performance.
2. **Event Sourcing**: Captures state changes as a sequence of immutable events.
3. **Domain-Driven Design (DDD)**: Structures the system to reflect the business domain.
4. **Apache Kafka**: Facilitates asynchronous communication between microservices.
5. **Event Versioning**: Manages changes in event definitions over time.
6. **Optimistic Concurrency Control**: Prevents conflicts in concurrent updates.
7. **Event Store Replay**: Rebuilds aggregates or read databases from historical events.
8. **Database-Per-Service Pattern**: Ensures independence between microservices.
9. **Entity Framework Core**: Simplifies database interactions.
10. **Docker**: Containerizes microservices for consistent deployment.
11. **Transactional Outbox Pattern**: Ensures atomicity between database operations and message publishing in distributed systems.

## Project Structure

The project is organized into the following modules:


1. **Command Handlers and Events Implementation**
   - Designing commands and events
   - Implementing the mediator pattern
   - Handling command execution and event raising

2. **Aggregates and Event Store**
   - Defining aggregates in a DDD context
   - Storing events in MongoDB
   - Retrieving and replaying events to rebuild aggregate state

3. **Read Database with MS SQL**
   - Creating a read model using Entity Framework Core
   - Populating the read database from event streams
   - Implementing query handlers

4. **Kafka for Inter-Service Communication**
   - Setting up Kafka as a message bus
   - Producing and consuming events in Kafka
   - Managing message serialization and deserialization
   - Implementing the Transactional Outbox Pattern

5. **Event Versioning and Concurrency**
   - Implementing versioning for events
   - Handling concurrency with optimistic locking
   - Managing changes in event schema

6. **Event Store Replay**
   - Techniques for replaying the event store
   - Rebuilding read databases in MS SQL and PostgreSQL
   - Ensuring consistency and integrity during replay

7. **Advanced Topics**
   - Database-Per-Service Pattern
   - Integration with Docker for microservices deployment
   - Testing strategies for CQRS and Event Sourcing


## Transactional Outbox Pattern

The Transactional Outbox Pattern is crucial for maintaining data consistency in distributed systems. It addresses the problem where a database operation succeeds, but the subsequent Kafka message production fails. Here's how we've implemented it:

1. **Outbox Table**: An `Outbox` table in our database stores messages that need to be published to Kafka.

2. **Atomic Transactions**: When handling a command that requires both database changes and message publishing, we:
   a. Perform the database operation
   b. Insert the message into the `Outbox` table
   c. Commit the transaction

3. **Message Relay**: A separate background process polls the `Outbox` table for unpublished messages.

4. **Publishing to Kafka**: The message relay publishes these messages to Kafka.

5. **Marking as Processed**: Successfully published messages are marked as processed in the `Outbox` table.

Sample Outbox table structure:

```sql
CREATE TABLE Outbox (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Topic NVARCHAR(255) NOT NULL,
    Key NVARCHAR(255),
    Message NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    ProcessedAt DATETIME2 NULL
);
This pattern significantly improves the reliability and consistency of our event-driven microservices architecture.

