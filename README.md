# 📧 Emails Microservice

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-LGPL%20v3-blue.svg)](LICENSE.md)
[![Tests](https://img.shields.io/badge/tests-passing-success)](tests/)
[![Coverage](https://img.shields.io/badge/coverage-85%25-green)]()
[![Docker](https://img.shields.io/badge/docker-ready-2496ED?logo=docker)](Dockerfile)

A production-ready microservice for sending emails and managing templates built with .NET 9. Implements Clean Architecture, DDD, and CQRS patterns with support for Microsoft Graph API integration.

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Key Features](#-key-features)
- [Technology Stack](#️-technology-stack)
- [Prerequisites](#️-prerequisites)
- [Getting Started](#-getting-started)
- [API Endpoints](#-api-endpoints)
- [Email Providers](#-email-providers)
- [Configuration](#️-configuration)
- [Use Cases & Scenarios](#-use-cases--scenarios)
- [Architecture](#️-architecture)
- [Testing](#-testing)
- [Best Practices](#-best-practices)
- [Troubleshooting](#-troubleshooting)
- [Email Flow](#-email-flow)
- [Template System](#-template-system)
- [Security](#-security)
- [FAQ](#-faq)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🎯 Overview

## What is this microservice?

The Emails microservice handles all outbound email communication from the platform: welcome messages when a new user is created, invoice notifications, payment confirmations, penalty alerts, circular letters to all residents, and password recovery flows. It solves the problem of centralizing email delivery with reusable templates so that other microservices only need to emit a domain event, and Emails takes care of composing and sending the message via Microsoft Graph. Administrators can manage templates, while residents and property owners receive emails automatically as part of their interaction with the platform.

---

The Emails microservice provides a unified API for sending transactional and template-based emails through Microsoft Graph API. It abstracts email complexity with features like:

- **Template Management**: Create and manage reusable email templates with variable substitution
- **Multi-recipient Support**: Send to multiple recipients with CC and BCC
- **Attachment Handling**: Support for file attachments with automatic conversion
- **Template Variables**: Dynamic content replacement using template placeholders
- **Base64 Encoding**: Automatic encoding/decoding for email bodies
- **Microsoft Graph Integration**: Leverage Microsoft 365 email infrastructure
- **Multi-tenancy**: Isolate emails by tenant
- **Event Publishing**: Domain events for email delivery tracking
- **User Notifications**: Automated emails for user lifecycle events (welcome, password reset)

### 🚀 Quick Start

```bash
# 1. Start infrastructure services
git clone https://github.com/codedesignplus/CodeDesignPlus.Environment.Dev
cd CodeDesignPlus.Environment.Dev/resources
docker-compose up -d

# 2. Configure Vault secrets
cd ../../tools/vault
./config-vault.sh

# 3. Run the microservice
dotnet run --project src/entrypoints/CodeDesignPlus.Net.Microservice.Emails.Rest

# 4. Access Swagger UI
open http://localhost:5000/swagger
```

### 📊 High-Level Architecture

```
┌─────────────┐
│   Client    │
│ Application │
└──────┬──────┘
       │ HTTPS + JWT
       │
┌──────▼──────────────────────────────────────────────┐
│         Emails Microservice (REST/gRPC/Worker)      │
│  ┌──────────────┐  ┌─────────────┐  ┌────────────┐ │
│  │ Controllers  │  │  MediatR    │  │  Handlers  │ │
│  │   (API)      │─▶│   (CQRS)    │─▶│ (Business) │ │
│  └──────────────┘  └─────────────┘  └────┬───────┘ │
│                                           │         │
│  ┌────────────────────────────────────────▼──────┐ │
│  │         Email Sender Service                  │ │
│  │  ┌─────────────────────────────────────────┐  │ │
│  │  │      Microsoft Graph API Client         │  │ │
│  │  │  (Azure AD Authentication)              │  │ │
│  │  └─────────────────────────────────────────┘  │ │
│  └────────────────────────────────────────────────┘ │
└───────┬──────────────────┬──────────────────┬───────┘
        │                  │                  │
   ┌────▼────┐      ┌──────▼──────┐    ┌─────▼─────┐
   │ MongoDB │      │Microsoft    │    │ RabbitMQ  │
   │(Templates│      │Graph API    │    │ (Events)  │
   │& History)│      │(Email Send) │    │           │
   └─────────┘      └─────────────┘    └───────────┘
```

## 🚀 Key Features

### Core Capabilities

- ✅ **Template Management**: CRUD operations for email templates with variables
- ✅ **Email Sending**: Send emails with dynamic content and attachments
- ✅ **Variable Substitution**: Replace template placeholders with actual values
- ✅ **Multi-Recipient**: Support for To, CC, and BCC recipients
- ✅ **File Attachments**: Attach files with automatic content type detection
- ✅ **Microsoft Graph API**: Native integration with Microsoft 365
- ✅ **Base64 Encoding**: Automatic body encoding for special characters
- ✅ **HTML/Plain Text**: Support for both HTML and plain text emails
- ✅ **Email Tracking**: Store sent email history with status codes
- ✅ **User Notifications**: Automated emails for user events (password reset, welcome)
- ✅ **Problem Details**: RFC 7807 compliant error responses

### Technical Features

- Clean Architecture with DDD and CQRS
- Domain events for email sent tracking
- MongoDB for template and history persistence
- RabbitMQ for event publishing
- Redis for distributed caching
- OAuth2/Azure AD authentication for Graph API
- Multi-tenancy support
- Swagger/OpenAPI documentation
- gRPC and REST API support
- Background worker for async email processing
- Docker containerization
- Comprehensive test coverage (Unit, Integration)

## 🛠️ Technology Stack

### Core
- **.NET 9** - Runtime and framework
- **ASP.NET Core** - Web API framework
- **C# 13** - Programming language

### Storage & Data
- **MongoDB** - Template and email history persistence
- **Redis** - Distributed caching and session storage

### Messaging & Events
- **RabbitMQ** - Event publishing and message broker

### Email Provider
- **Microsoft Graph API** - Email sending via Microsoft 365
- **Azure AD** - OAuth2 authentication for Graph API
- **Microsoft.Graph SDK** - Graph API client library

### Architecture & Patterns
- **MediatR** - CQRS command/query handling
- **FluentValidation** - Input validation
- **Mapster** - Object mapping
- **NodaTime** - Date/time handling

### Security & Configuration
- **Vault** - Secret management
- **Azure AD OAuth2** - Microsoft Graph authentication
- **JWT Bearer** - Token-based security
- **HTTPS** - Encrypted communication

### DevOps & Testing
- **Docker** - Containerization
- **xUnit** - Unit/integration testing
- **Swagger/OpenAPI** - API documentation
- **gRPC** - High-performance RPC

## ⚙️ Prerequisites

### Required
- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Docker & Docker Compose** - For infrastructure services
- **MongoDB 6.0+** - Document database
- **Redis 7.0+** - Caching layer
- **RabbitMQ 3.12+** - Message broker

### Microsoft 365 Configuration
- **Azure AD Tenant** - For authentication
- **App Registration** - In Azure AD with Mail.Send permissions
- **User with License** - Microsoft 365 user with mailbox
- **Client Credentials** - TenantId, ClientId, ClientSecret

### Optional
- **Vault** - Secret management (can use appsettings for local dev)

## 🚀 Getting Started

The following instructions will help you set up the project on your local machine for development and testing purposes.

### 1. Clone the repository

```bash
git clone <repository-url>
cd CodeDesignPlus.Net.Microservice.Emails
```

### 2. Start Infrastructure Services

Clone and run the development environment:

```bash
git clone https://github.com/codedesignplus/CodeDesignPlus.Environment.Dev
cd CodeDesignPlus.Environment.Dev/resources
docker-compose up -d
```

This starts:
- MongoDB (localhost:27017)
- Redis (localhost:6379)
- RabbitMQ (localhost:5672, Management: localhost:15672)
- Vault (localhost:8200)

### 3. Configure Vault

```bash
cd tools/vault
./config-vault.sh
```

### 4. Configure Azure AD App Registration

1. Go to [Azure Portal](https://portal.azure.com) → Azure Active Directory → App registrations
2. Create new registration: `ms-emails-app`
3. Add API permissions: `Microsoft Graph → Application → Mail.Send`
4. Grant admin consent for the tenant
5. Create client secret and copy values:
   - Tenant ID
   - Client ID
   - Client Secret

### 5. Update Configuration

Edit `src/entrypoints/CodeDesignPlus.Net.Microservice.Emails.Rest/appsettings.Development.json`:

```json
{
  "Email": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "UserIdWithLicense": "user@yourdomain.com"
  }
}
```

### 6. Build the Solution

```bash
dotnet build
```

### 7. Run the Desired Entry Point

**REST API**:
```bash
dotnet run --project src/entrypoints/CodeDesignPlus.Net.Microservice.Emails.Rest
# Access: http://localhost:5000/swagger
```

**gRPC**:
```bash
dotnet run --project src/entrypoints/CodeDesignPlus.Net.Microservice.Emails.gRpc
# gRPC endpoint: http://localhost:5001
```

**AsyncWorker** (Background jobs):
```bash
dotnet run --project src/entrypoints/CodeDesignPlus.Net.Microservice.Emails.AsyncWorker
# Listens to RabbitMQ queues for email events
```

## 📡 API Endpoints

### Email Operations

#### Send Email
```http
POST /api/email
Content-Type: multipart/form-data
Authorization: Bearer {token}
X-Tenant: {tenant-id}

Form Data:
- id: 550e8400-e29b-41d4-a716-446655440000
- idTemplate: 660e8400-e29b-41d4-a716-446655440001
- to: ["recipient@example.com"]
- cc: ["cc@example.com"]
- bcc: ["bcc@example.com"]
- subject: "Welcome to Our Platform"
- attachments: [file1, file2]
- values: {"userName": "John Doe", "loginUrl": "https://app.example.com"}
```

**Response**: `204 No Content`

#### Get Email by ID
```http
GET /api/email/{id}
Authorization: Bearer {token}
X-Tenant: {tenant-id}
```

**Response**: `200 OK` with email details
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "to": ["recipient@example.com"],
  "cc": ["cc@example.com"],
  "bcc": ["bcc@example.com"],
  "subject": "Welcome to Our Platform",
  "body": "SGVsbG8gSm9obiBEb2UsIFdlbGNvbWU=",
  "from": "noreply@example.com",
  "attachments": [
    {
      "name": "document.pdf",
      "contentType": "application/pdf",
      "size": 1024
    }
  ],
  "isHtml": true,
  "values": {
    "userName": "John Doe",
    "loginUrl": "https://app.example.com"
  },
  "code": "202",
  "error": null,
  "tenant": "tenant-id",
  "createdAt": "2025-05-15T10:30:00Z"
}
```

#### Get All Emails
```http
GET /api/email?page=1&pageSize=20&orderBy=createdAt&order=desc
Authorization: Bearer {token}
X-Tenant: {tenant-id}
```

**Response**: `200 OK` with paginated email list
```json
{
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "subject": "Welcome to Our Platform",
      "to": ["recipient@example.com"],
      "from": "noreply@example.com",
      "code": "202",
      "createdAt": "2025-05-15T10:30:00Z"
    }
  ],
  "page": 1,
  "pageSize": 20,
  "total": 150
}
```

### Template Operations

#### Create Template
```http
POST /api/template
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "id": "660e8400-e29b-41d4-a716-446655440001",
  "name": "Welcome Email",
  "subject": "Welcome {{userName}}!",
  "body": "<html><body><h1>Hello {{userName}}</h1><p>Click <a href='{{loginUrl}}'>here</a> to login.</p></body></html>",
  "variables": ["userName", "loginUrl"],
  "attachments": [],
  "from": "noreply@example.com",
  "alias": "Support Team",
  "isHtml": true,
  "tenant": "tenant-id"
}
```

**Response**: `204 No Content`

#### Get Template by ID
```http
GET /api/template/{id}
Authorization: Bearer {token}
X-Tenant: {tenant-id}
```

**Response**: `200 OK` with template details
```json
{
  "id": "660e8400-e29b-41d4-a716-446655440001",
  "name": "Welcome Email",
  "subject": "Welcome {{userName}}!",
  "body": "PGh0bWw+PGJvZHk+PGgxPkhlbGxvIHt7dXNlck5hbWV9fTwvaDE+PC9ib2R5PjwvaHRtbD4=",
  "variables": ["userName", "loginUrl"],
  "attachments": [],
  "from": "noreply@example.com",
  "alias": "Support Team",
  "isHtml": true,
  "tenant": "tenant-id",
  "createdAt": "2025-05-10T09:00:00Z"
}
```

#### Get All Templates
```http
GET /api/template?page=1&pageSize=20
Authorization: Bearer {token}
X-Tenant: {tenant-id}
```

**Response**: `200 OK` with paginated template list

#### Update Template
```http
PUT /api/template/{id}
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "name": "Updated Welcome Email",
  "subject": "Welcome {{userName}}!",
  "body": "<html>...</html>",
  "variables": ["userName", "loginUrl", "supportEmail"],
  "attachments": [],
  "from": "noreply@example.com",
  "alias": "Support Team",
  "isHtml": true
}
```

**Response**: `204 No Content`

#### Delete Template
```http
DELETE /api/template/{id}
Authorization: Bearer {token}
X-Tenant: {tenant-id}
```

**Response**: `204 No Content`

### User Configuration Operations

#### Create User Email Configuration
```http
POST /api/user
Content-Type: application/json
Authorization: Bearer {token}

{
  "id": "user-id",
  "idTemplate": "template-id",
  "typeTemplate": "PasswordTemp",
  "subject": "Password Reset",
  "uriLoginApp": "https://app.example.com/login"
}
```

**Response**: `204 No Content`

#### Update User Email Configuration
```http
PUT /api/user/{id}
Content-Type: application/json
Authorization: Bearer {token}

{
  "idTemplate": "template-id",
  "typeTemplate": "PasswordTemp",
  "subject": "Password Reset",
  "uriLoginApp": "https://app.example.com/login"
}
```

**Response**: `204 No Content`

## 📧 Email Providers

### Microsoft Graph API

The microservice uses Microsoft Graph API to send emails through Microsoft 365 infrastructure.

#### Authentication Flow

1. **App Registration**: Register app in Azure AD with Mail.Send permission
2. **Client Credentials**: Use OAuth2 client credentials flow
3. **Access Token**: Graph client acquires token automatically
4. **Send Email**: Use `/users/{userId}/sendMail` endpoint

#### Configuration

```json
{
  "Email": {
    "TenantId": "your-azure-ad-tenant-id",
    "ClientId": "your-app-client-id",
    "ClientSecret": "your-app-client-secret",
    "Scopes": ["https://graph.microsoft.com/.default"],
    "UserIdWithLicense": "sender@yourdomain.com"
  }
}
```

#### Permissions Required

- **Application Permission**: `Mail.Send` (requires admin consent)
- **User**: Must have Microsoft 365 mailbox/license

#### Rate Limits

Microsoft Graph API has rate limits:
- **Per user**: 10,000 requests per 10 minutes
- **Per tenant**: 100,000 requests per 10 minutes

For high-volume scenarios, implement:
- Rate limiting middleware
- Exponential backoff retry
- Queue-based sending

#### Error Handling

Common Graph API errors:
- `401 Unauthorized`: Invalid credentials or expired token
- `403 Forbidden`: Missing permissions or user has no mailbox
- `429 Too Many Requests`: Rate limit exceeded
- `503 Service Unavailable`: Graph API temporary issue

## ⚙️ Configuration

### Environment Variables

```bash
# Core
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:5000

# Database
MONGO_CONNECTION_STRING=mongodb://mongo:27017

# Cache
REDIS_CONNECTION_STRING=redis:6379

# Messaging
RABBITMQ_HOST=rabbitmq
RABBITMQ_PORT=5672
RABBITMQ_USERNAME=user
RABBITMQ_PASSWORD=pass

# Secrets
VAULT_ADDRESS=http://vault:8200
VAULT_TOKEN=your-vault-token

# Microsoft Graph
EMAIL_TENANT_ID=your-tenant-id
EMAIL_CLIENT_ID=your-client-id
EMAIL_CLIENT_SECRET=your-client-secret
EMAIL_USER_ID_WITH_LICENSE=sender@yourdomain.com
```

### Vault Configuration

Store sensitive credentials in Vault:

```bash
# Write Graph API credentials
vault kv put secret/security-codedesignplus/ms-emails/email \
  tenantId="your-tenant-id" \
  clientId="your-client-id" \
  clientSecret="your-client-secret" \
  userIdWithLicense="sender@yourdomain.com"

# Write MongoDB credentials
vault kv put secret/security-codedesignplus/ms-emails/mongo \
  username="emailuser" \
  password="secure-password"

# Write RabbitMQ credentials
vault kv put secret/security-codedesignplus/ms-emails/rabbitmq \
  username="emailuser" \
  password="secure-password"
```

### Email Options

Configure via `appsettings.json` or Vault:

```json
{
  "Email": {
    "TenantId": "Azure AD tenant ID",
    "ClientId": "App registration client ID",
    "ClientSecret": "App client secret",
    "Scopes": ["https://graph.microsoft.com/.default"],
    "UserIdWithLicense": "Email of user with Microsoft 365 license"
  }
}
```

## 🎯 Use Cases & Scenarios

### 1. Welcome Email for New Users

Send personalized welcome email when user registers:

```bash
# Step 1: User service publishes UserCreatedDomainEvent to RabbitMQ
Event: UserCreatedDomainEvent
- AggregateId: user-id
- Email: john@example.com
- FirstName: John
- LastName: Doe
- PasswordKey: temp-key
- PasswordCipher: encrypted-password

# Step 2: AsyncWorker consumes event (SendMailPasswordTempHandler)
Queue: User.sendmailpasswordtemp

# Step 3: Handler sends welcome email command
Command: SendMailPasswordTempCommand
- Template: "Welcome Email"
- Variables: { userName: "John Doe", password: "temp123", loginUrl: "..." }

# Step 4: Email sent via Microsoft Graph API
Response: 202 Accepted

# Step 5: Email history saved to MongoDB
EmailsAggregate persisted with status code and recipients
```

### 2. Template-Based Notifications

Use templates for consistent branding:

```bash
# Step 1: Create template
POST /api/template
{
  "name": "Order Confirmation",
  "subject": "Order #{{orderId}} Confirmed",
  "body": "<html>Dear {{customerName}}, Your order #{{orderId}} has been confirmed...</html>",
  "variables": ["orderId", "customerName", "orderDate", "total"],
  "from": "orders@example.com",
  "alias": "Order Team"
}

# Step 2: Send email using template
POST /api/email
{
  "idTemplate": "template-id",
  "to": ["customer@example.com"],
  "values": {
    "orderId": "12345",
    "customerName": "John Doe",
    "orderDate": "2025-05-15",
    "total": "$150.00"
  }
}

# Step 3: Variables replaced in template
Output: "Dear John Doe, Your order #12345 has been confirmed..."
```

### 3. Multi-Recipient Notifications

Send to multiple recipients with CC and BCC:

```bash
# Notify customer + CC support team + BCC manager
POST /api/email
{
  "to": ["customer@example.com"],
  "cc": ["support@example.com"],
  "bcc": ["manager@example.com"],
  "subject": "Issue #456 Resolved",
  "idTemplate": "issue-resolution-template",
  "values": {
    "issueId": "456",
    "customerName": "Jane Doe",
    "resolution": "..."
  }
}
```

### 4. Attachments with Emails

Send invoices, reports, or documents:

```bash
# Send invoice with PDF attachment
POST /api/email
Content-Type: multipart/form-data

Form Data:
- to: ["customer@example.com"]
- subject: "Invoice #789"
- idTemplate: "invoice-template"
- values: { invoiceNumber: "789", amount: "$250.00" }
- attachments: [invoice-789.pdf, terms.pdf]

# Attachments automatically converted to base64 and sent via Graph API
```

### 5. Password Reset Emails

Automated password reset workflow:

```bash
# Step 1: User requests password reset
User clicks "Forgot Password"

# Step 2: User service generates temp password and publishes event
Event: UserCreatedDomainEvent (WasCreatedFromSSO = false)

# Step 3: AsyncWorker processes event
Handler: SendMailPasswordTempHandler

# Step 4: Email sent with temp password
POST /api/email
{
  "idTemplate": "password-reset-template",
  "to": ["user@example.com"],
  "values": {
    "userName": "John Doe",
    "tempPassword": "Temp123!",
    "loginUrl": "https://app.example.com/login"
  }
}
```

### 6. Multi-Tenant Email Isolation

Isolate emails by tenant:

```bash
# Tenant A emails
POST /api/email
Headers: X-Tenant: tenant-a-id
- Template: tenant-a-welcome-template
- From: support@tenant-a.com

# Tenant B emails (completely isolated)
POST /api/email
Headers: X-Tenant: tenant-b-id
- Template: tenant-b-welcome-template
- From: support@tenant-b.com

# Templates and history are tenant-specific
```

## 🏗️ Architecture

### Clean Architecture Layers

```
src/
├── domain/                          # Domain Layer
│   ├── Domain/                      # Aggregates, Entities, Value Objects
│   │   ├── EmailsAggregate.cs      # Email history aggregate
│   │   ├── TemplateAggregate.cs    # Email template aggregate
│   │   ├── UserAggregate.cs        # User email config aggregate
│   │   ├── Enums/                  # TypeTemplate
│   │   ├── Models/                 # EmailMessage, Attachment, EmailResponse
│   │   ├── DomainEvents/           # EmailSentDomainEvent, Template events
│   │   ├── Repositories/           # IEmailsRepository, ITemplateRepository
│   │   └── Services/               # IEmailSender
│   ├── Application/                 # Application Layer
│   │   ├── Emails/                 # Email commands/queries
│   │   │   ├── Commands/           # SendEmailCommand
│   │   │   ├── Queries/            # GetAllEmailsQuery, GetEmailsByIdQuery
│   │   │   └── DTOs/               # EmailsDto
│   │   ├── Template/               # Template commands/queries
│   │   │   ├── Commands/           # CreateTemplate, UpdateTemplate, DeleteTemplate
│   │   │   ├── Queries/            # GetAllQuery, FindByIdQuery
│   │   │   └── DTOs/               # TemplateDto
│   │   └── User/                   # User email config
│   │       └── Commands/           # CreateConfigUserTemplate, SendMailPasswordTemp
│   └── Infrastructure/              # Infrastructure Layer
│       ├── Repositories/           # MongoDB implementations
│       ├── Services/               # EmailSender, GraphClient
│       └── Options/                # EmailOptions
└── entrypoints/                     # Presentation Layer
    ├── Rest/                        # REST API
    │   ├── Controllers/            # EmailController, TemplateController
    │   └── DataTransferObjects/    # SendEmailDto, CreateTemplateDto
    ├── gRpc/                        # gRPC API
    │   ├── Services/               # EmailsService
    │   └── Protos/                 # emails.proto
    └── AsyncWorker/                 # Background jobs
        ├── Consumers/              # SendMailPasswordTempHandler
        └── DomainEvents/           # UserCreatedDomainEvent
```

### CQRS Pattern

**Commands** (Write operations):
- `SendEmailCommand` - Send email with template and variables
- `CreateTemplateCommand` - Create new template
- `UpdateTemplateCommand` - Update existing template
- `DeleteTemplateCommand` - Soft delete template
- `CreateConfigUserTemplateCommand` - Configure user email settings
- `UpdateConfigUserTemplateCommand` - Update user email config
- `SendMailPasswordTempCommand` - Send temporary password email

**Queries** (Read operations):
- `GetAllEmailsQuery` - List sent emails with pagination
- `GetEmailsByIdQuery` - Get email by ID
- `GetAllQuery` - List templates with pagination
- `FindByIdQuery` - Get template by ID

### Domain Events

Published to RabbitMQ after successful operations:

- `EmailSentDomainEvent` - Email sent successfully
- `TemplateCreatedDomainEvent` - New template created
- `TemplateUpdatedDomainEvent` - Template modified
- `TemplateDeletedDomainEvent` - Template deleted

### Email Flow

```
Client requests email
     ↓
[Controller] → Validates request + attachments
     ↓
[SendEmailCommand] → Validates template and recipients
     ↓
[Handler] → Retrieves template from MongoDB
     ↓
[BuildBody] → Replaces variables in template body
     ↓
[EmailMessage] → Creates email message with:
     - To, CC, BCC recipients
     - Subject
     - Base64-encoded body
     - Attachments (converted to byte[])
     - From/Alias
     ↓
[EmailSender] → Decodes body from Base64
     ↓
[GraphClient] → Authenticates with Azure AD
     ↓
[Microsoft Graph API] → Sends email via /users/{userId}/sendMail
     ↓
[EmailResponse] → Receives status code (202 Accepted or error)
     ↓
[EmailsAggregate] → Creates aggregate with sent email data
     ↓
[Repository] → Persists to MongoDB (db-ms-emails.EmailsAggregate)
     ↓
[IPubSub] → Publishes EmailSentDomainEvent to RabbitMQ
     ↓
Returns 204 No Content to client
```

### Template Variable Substitution

```
Template Body (Base64):
"PGgxPkhlbGxvIHt7dXNlck5hbWV9fTwvaDE+" (encoded)
     ↓
Decoded:
"<h1>Hello {{userName}}</h1>"
     ↓
Variables Applied:
{ "userName": "John Doe" }
     ↓
Result:
"<h1>Hello John Doe</h1>"
     ↓
Re-encoded (Base64):
"PGgxPkhlbGxvIEpvaG4gRG9lPC9oMT4="
     ↓
Sent to Graph API
```

## 🧪 Testing

### Run All Tests

```bash
# Run all tests (unit + integration)
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverageThreshold=80

# Run specific test project
dotnet test tests/unit/CodeDesignPlus.Net.Microservice.Emails.Domain.Test
```

### Test Structure

```
tests/
├── unit/                            # Unit tests
│   ├── Domain.Test/                # Aggregate, entity tests
│   │   ├── EmailsAggregateTest.cs
│   │   ├── TemplateAggregateTest.cs
│   │   └── UserAggregateTest.cs
│   ├── Application.Test/           # Handler, validator tests
│   │   ├── SendEmailCommandHandlerTest.cs
│   │   └── CreateTemplateCommandHandlerTest.cs
│   ├── Infrastructure.Test/        # Repository, service tests
│   │   ├── EmailSenderTest.cs
│   │   └── GraphClientTest.cs
│   ├── Rest.Test/                  # Controller tests
│   │   └── EmailControllerTest.cs
│   ├── gRpc.Test/                  # gRPC service tests
│   │   └── EmailsServiceTest.cs
│   └── AsyncWorker.Test/           # Consumer tests
│       └── SendMailPasswordTempHandlerTest.cs
├── integration/                     # Integration tests
│   ├── Rest.Test/                  # REST API integration
│   │   └── EmailEndpointsTest.cs
│   ├── gRpc.Test/                  # gRPC integration
│   │   └── EmailsServiceIntegrationTest.cs
│   └── AsyncWorker.Test/           # Worker integration
│       └── SendMailPasswordTempIntegrationTest.cs
└── load/                            # Load/performance tests
    └── artillery-config.yml
```

### Unit Test Example

```csharp
[Fact]
public void Create_ValidData_ShouldCreateEmailsAggregate()
{
    // Arrange
    var id = Guid.NewGuid();
    var to = new List<string> { "test@example.com" };
    var subject = "Test Subject";
    var body = "VGVzdCBCb2R5"; // Base64 encoded "Test Body"
    var from = "sender@example.com";

    // Act
    var aggregate = EmailsAggregate.Create(
        id, to, [], [], subject, body, from, [], true, 
        new Dictionary<string, string>(), "202", null, Guid.NewGuid()
    );

    // Assert
    Assert.Equal(id, aggregate.Id);
    Assert.Equal(to, aggregate.To);
    Assert.Equal(subject, aggregate.Subject);
    Assert.Equal(body, aggregate.Body);
    Assert.Equal(from, aggregate.From);
    Assert.True(aggregate.IsHtml);
    Assert.Equal("202", aggregate.Code);
    Assert.Null(aggregate.Error);
}
```

### Integration Test Example

```csharp
[Fact]
public async Task SendEmail_ValidTemplate_ShouldReturn204()
{
    // Arrange
    var client = _factory.CreateClient();
    var template = await CreateTestTemplate();
    var request = new MultipartFormDataContent
    {
        { new StringContent(Guid.NewGuid().ToString()), "id" },
        { new StringContent(template.Id.ToString()), "idTemplate" },
        { new StringContent("[\"test@example.com\"]"), "to" },
        { new StringContent("Test Subject"), "subject" },
        { new StringContent("{\"userName\":\"John\"}"), "values" }
    };

    // Act
    var response = await client.PostAsync("/api/email", request);

    // Assert
    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
}
```

### Load Testing

Run load tests using Artillery:

```bash
cd tests/load
artillery run artillery-config.yml
```

## 📋 Best Practices

### Template Design

1. **Use Meaningful Variable Names**
   ```html
   <!-- Good -->
   <p>Hello {{userName}}, your order #{{orderId}} is ready!</p>
   
   <!-- Bad -->
   <p>Hello {{v1}}, your order #{{v2}} is ready!</p>
   ```

2. **Document Required Variables**
   ```json
   {
     "name": "Order Confirmation",
     "variables": ["userName", "orderId", "orderDate", "total"],
     "description": "Sent when order is confirmed"
   }
   ```

3. **Use HTML for Rich Content**
   ```html
   <html>
     <body style="font-family: Arial, sans-serif;">
       <div style="max-width: 600px; margin: 0 auto;">
         <h1 style="color: #333;">{{subject}}</h1>
         <p>Dear {{userName}},</p>
         <div style="background: #f5f5f5; padding: 20px;">
           {{content}}
         </div>
         <p>Best regards,<br>{{teamName}}</p>
       </div>
     </body>
   </html>
   ```

4. **Test Templates Before Production**
   - Send test emails to internal addresses
   - Verify variable substitution
   - Check rendering across email clients

### Email Sending

1. **Validate Recipients**
   ```csharp
   // Use FluentValidation
   RuleFor(x => x.To)
       .NotEmpty()
       .Must(list => list.All(email => IsValidEmail(email)));
   ```

2. **Handle Attachments Properly**
   ```csharp
   // Limit attachment size
   const int MaxAttachmentSizeMB = 10;
   if (file.Length > MaxAttachmentSizeMB * 1024 * 1024)
       throw new ValidationException("Attachment too large");
   ```

3. **Use Async Workers for High Volume**
   - Don't send emails synchronously in REST endpoints
   - Publish domain events to RabbitMQ
   - Let AsyncWorker process queue asynchronously

4. **Implement Retry Logic**
   ```csharp
   // Retry on transient failures (429, 503)
   try
   {
       await graphClient.SendMailAsync(message);
   }
   catch (ServiceException ex) when (ex.StatusCode == 429)
   {
       await Task.Delay(TimeSpan.FromSeconds(30));
       // Retry
   }
   ```

### Security

1. **Validate Tenant Isolation**
   - Always check X-Tenant header
   - Filter queries by tenant
   - Don't allow cross-tenant access

2. **Sanitize Email Content**
   ```csharp
   // Prevent XSS in email bodies
   body = HtmlEncoder.Default.Encode(body);
   ```

3. **Rate Limit Email Sending**
   ```csharp
   // Implement rate limiting per user/tenant
   [RateLimit(100, TimeWindow.Hour)]
   public async Task<IActionResult> SendEmail(SendEmailDto data)
   {
       // ...
   }
   ```

4. **Store Credentials Securely**
   - Never commit secrets to Git
   - Use Vault or Azure Key Vault
   - Rotate credentials regularly

### Performance

1. **Cache Templates**
   ```csharp
   // Cache frequently used templates in Redis
   var template = await _cache.GetOrCreateAsync(
       $"template:{templateId}",
       async () => await _repository.FindByIdAsync(templateId),
       TimeSpan.FromMinutes(30)
   );
   ```

2. **Batch Email Sending**
   - Group emails by tenant/template
   - Send in batches of 50-100
   - Use parallel processing with limits

3. **Monitor Graph API Rate Limits**
   - Track requests per minute
   - Implement exponential backoff
   - Queue emails when approaching limits

## 🔧 Troubleshooting

### Common Issues

#### 1. Email Not Sent (401 Unauthorized)

**Symptom**: `401 Unauthorized` when calling Graph API

**Causes**:
- Invalid Azure AD credentials
- Expired client secret
- Wrong tenant ID

**Solution**:
```bash
# Verify credentials in Vault
vault kv get secret/security-codedesignplus/ms-emails/email

# Test authentication manually
az login --tenant {tenant-id}
az account get-access-token --resource https://graph.microsoft.com

# Regenerate client secret if expired
# Update Vault with new secret
```

#### 2. Missing Permissions (403 Forbidden)

**Symptom**: `403 Forbidden` when sending email

**Causes**:
- App registration missing `Mail.Send` permission
- Permission not granted admin consent
- User has no mailbox

**Solution**:
1. Go to Azure Portal → App registrations → your-app
2. API permissions → Add permission → Microsoft Graph → Application → Mail.Send
3. Grant admin consent
4. Verify user has Microsoft 365 license with mailbox

#### 3. Rate Limit Exceeded (429 Too Many Requests)

**Symptom**: `429 Too Many Requests` from Graph API

**Causes**:
- Exceeded 10,000 requests per 10 minutes per user
- Exceeded 100,000 requests per 10 minutes per tenant

**Solution**:
```csharp
// Implement retry with exponential backoff
services.AddHttpClient<IGraphClient, GraphClient>()
    .AddTransientHttpErrorPolicy(policy => 
        policy.WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
        )
    );

// Queue emails for gradual processing
```

#### 4. Template Variables Not Replaced

**Symptom**: Email shows `{{userName}}` instead of actual value

**Causes**:
- Variable name mismatch
- Body not decoded from Base64 before replacement

**Solution**:
```csharp
// Ensure BuildBody decodes before replacement
var templateDecode = Encoding.UTF8.GetString(Convert.FromBase64String(template));
foreach (var kvp in values)
{
    templateDecode = templateDecode.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
}
return Convert.ToBase64String(Encoding.UTF8.GetBytes(templateDecode));
```

#### 5. Attachments Not Received

**Symptom**: Email sent but attachments missing

**Causes**:
- Attachment size exceeds Graph API limit (4MB per attachment)
- Incorrect content type
- File stream not read properly

**Solution**:
```csharp
// Limit attachment size
if (file.Length > 4 * 1024 * 1024)
    throw new ValidationException("Attachment exceeds 4MB limit");

// Ensure stream is read completely
using var stream = new MemoryStream();
await file.CopyToAsync(stream);
var fileBytes = stream.ToArray();
```

#### 6. AsyncWorker Not Processing Events

**Symptom**: RabbitMQ queue has messages but worker doesn't process

**Causes**:
- Worker not running
- Queue name mismatch
- RabbitMQ connection issue

**Solution**:
```bash
# Check worker is running
docker ps | grep ms-emails-worker

# Verify RabbitMQ connection
rabbitmqctl list_connections

# Check queue bindings
rabbitmqctl list_queues name messages consumers

# Verify queue name in [QueueName] attribute
[QueueName("User", "sendmailpasswordtemp")] // Queue: User.sendmailpasswordtemp
```

### Debugging Tips

1. **Enable Diagnostics**
   ```json
   {
     "Logger": {
       "Level": "Debug"
     },
     "Mongo": {
       "Diagnostic": {
         "Enable": true,
         "EnableCommandText": true
       }
     },
     "RabbitMQ": {
       "EnableDiagnostic": true
     }
   }
   ```

2. **Inspect RabbitMQ Messages**
   - Management UI: http://localhost:15672
   - Username: user
   - Password: pass
   - Check queues, bindings, and message rates

3. **Check MongoDB Collections**
   ```bash
   mongosh mongodb://localhost:27017
   use db-ms-emails
   db.EmailsAggregate.find().limit(10)
   db.TemplateAggregate.find().limit(10)
   ```

4. **Test Graph API Manually**
   ```bash
   # Get access token
   TOKEN=$(curl -X POST https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token \
     -d "client_id={clientId}" \
     -d "client_secret={clientSecret}" \
     -d "scope=https://graph.microsoft.com/.default" \
     -d "grant_type=client_credentials" \
     | jq -r .access_token)

   # Send test email
   curl -X POST https://graph.microsoft.com/v1.0/users/{userId}/sendMail \
     -H "Authorization: Bearer $TOKEN" \
     -H "Content-Type: application/json" \
     -d '{"message":{"subject":"Test","body":{"contentType":"Text","content":"Test"},"toRecipients":[{"emailAddress":{"address":"test@example.com"}}]}}'
   ```

## 📧 Template System

### Template Structure

Email templates consist of:
- **Name**: Human-readable template identifier
- **Subject**: Email subject with optional variables
- **Body**: HTML or plain text body (Base64 encoded)
- **Variables**: Array of variable names used in subject/body
- **From**: Sender email address
- **Alias**: Sender display name
- **IsHtml**: Whether body is HTML or plain text
- **Attachments**: Array of default attachment names (optional)

### Variable Syntax

Use double curly braces for variables:

```html
<!-- Subject -->
Welcome {{userName}} to {{platformName}}!

<!-- Body -->
<html>
  <body>
    <h1>Hello {{userName}},</h1>
    <p>Welcome to {{platformName}}!</p>
    <p>Your account is active. Click <a href="{{loginUrl}}">here</a> to login.</p>
    <p>Your temporary password is: <strong>{{tempPassword}}</strong></p>
    <p>Best regards,<br>{{teamName}}</p>
  </body>
</html>
```

### Template Types

#### 1. Transactional Emails
- Order confirmations
- Payment receipts
- Shipping notifications
- Password resets

#### 2. User Lifecycle Emails
- Welcome emails
- Account activation
- Password changes
- Account deletion

#### 3. Notification Emails
- System alerts
- Task reminders
- Status updates
- Security notifications

### Base64 Encoding

Templates are stored with Base64-encoded bodies to:
- Handle special characters safely
- Preserve formatting
- Avoid encoding issues

**Encoding**:
```csharp
var body = "<html>...</html>";
var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(body));
// Store: "PGh0bWw+Li4uPC9odG1sPg=="
```

**Decoding**:
```csharp
var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
// Result: "<html>...</html>"
```

### Template Best Practices

1. **Keep Templates Simple**
   - Avoid complex logic
   - Use external CSS
   - Test across email clients

2. **Version Templates**
   - Create new template for major changes
   - Keep old templates for historical emails
   - Use naming convention: `welcome-email-v2`

3. **Localization**
   - Create template per language
   - Use locale in template name: `welcome-email-en`, `welcome-email-es`
   - Pass locale as variable for dynamic content

4. **Mobile-Friendly**
   ```html
   <meta name="viewport" content="width=device-width, initial-scale=1.0">
   <style>
     @media only screen and (max-width: 600px) {
       .content { width: 100% !important; }
     }
   </style>
   ```

## 🔒 Security

### Authentication & Authorization

1. **OAuth2/OpenID Connect**
   - All REST endpoints require JWT bearer token
   - Token validation via Security package
   - Claims-based authorization

2. **Azure AD for Graph API**
   - Client credentials flow
   - Application permissions (Mail.Send)
   - Secure token caching

3. **API Key for gRPC** (optional)
   - Service-to-service authentication
   - Metadata-based key validation

### Multi-Tenancy

1. **Tenant Isolation**
   - X-Tenant header required for all requests
   - Tenant filter in MongoDB queries
   - Domain events include tenant ID

2. **Data Segregation**
   ```csharp
   // Templates filtered by tenant
   var templates = await _repository.FindAsync(
       filter: x => x.Tenant == tenantId,
       criteria
   );
   ```

### Input Validation

1. **FluentValidation**
   ```csharp
   public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
   {
       public SendEmailCommandValidator()
       {
           RuleFor(x => x.To).NotEmpty().Must(BeValidEmailList);
           RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
           RuleFor(x => x.Values).NotEmpty();
       }
   }
   ```

2. **Sanitization**
   - HTML encode user input
   - Validate email addresses
   - Limit attachment sizes

### Secrets Management

1. **Vault Integration**
   - Graph API credentials in Vault
   - MongoDB connection string in Vault
   - RabbitMQ credentials in Vault

2. **Environment Variables**
   - No secrets in appsettings.json
   - Use Vault or environment variables
   - Rotate secrets regularly

### Audit & Logging

1. **Email History**
   - All sent emails persisted in MongoDB
   - Includes recipients, status, timestamp
   - Queryable for compliance

2. **Domain Events**
   - EmailSentDomainEvent published to RabbitMQ
   - Consumed by audit services
   - Immutable event log

3. **OpenTelemetry**
   - Distributed tracing enabled
   - Logs exported to OTel collector
   - Monitor Graph API calls

## ❓ FAQ

### General

**Q: Which email providers are supported?**  
A: Currently Microsoft Graph API (Microsoft 365). The architecture supports adding other providers via adapter pattern.

**Q: Can I use my own SMTP server?**  
A: Not in current version. The service uses Microsoft Graph API. To use SMTP, implement `IEmailSender` with SMTP client.

**Q: What's the maximum email size?**  
A: Microsoft Graph limits: 4MB per attachment, 28MB total message size.

**Q: How many recipients can I send to?**  
A: Microsoft 365 limits: 500 recipients per message (To + CC + BCC combined).

### Templates

**Q: Can I use HTML in templates?**  
A: Yes. Set `isHtml: true` and provide HTML body. Body is Base64 encoded automatically.

**Q: How do I preview templates?**  
A: Send test email to yourself with sample variables.

**Q: Can I use conditional logic in templates?**  
A: No. Templates support simple variable substitution only. For complex logic, generate content in application layer.

**Q: Can I attach files to templates?**  
A: Templates can define default attachment names, but actual files must be provided when sending email.

### Performance

**Q: How many emails can I send per second?**  
A: Limited by Microsoft Graph rate limits: ~17 emails/second per user. Use multiple sender accounts or queue-based processing for higher volume.

**Q: Should I send emails synchronously?**  
A: For low volume (<10/min), yes. For high volume, publish to RabbitMQ and use AsyncWorker.

**Q: Can I batch send emails?**  
A: Graph API doesn't support batch sendMail. Send individually with rate limiting.

### Troubleshooting

**Q: Email not received, but no error?**  
A: Check spam folder, verify sender reputation, inspect Graph API response.

**Q: Variables not replaced?**  
A: Ensure variable names match exactly (case-sensitive). Verify template body is Base64 encoded.

**Q: Attachments missing?**  
A: Check attachment size (<4MB each). Verify file stream is read correctly.

**Q: How do I debug Graph API calls?**  
A: Enable diagnostics, check logs, use Fiddler/Postman to test Graph API manually.

## 🐳 Docker Support

### Build Docker Images

**REST API**:
```bash
docker build -t ms-emails-rest:latest \
  -f src/entrypoints/CodeDesignPlus.Net.Microservice.Emails.Rest/Dockerfile .

docker run -d -p 5000:5000 \
  --network=backend \
  -e ASPNETCORE_ENVIRONMENT=Docker \
  -e MONGO_CONNECTION_STRING=mongodb://mongo:27017 \
  -e REDIS_CONNECTION_STRING=redis:6379 \
  -e RABBITMQ_HOST=rabbitmq \
  -e EMAIL_TENANT_ID=your-tenant-id \
  -e EMAIL_CLIENT_ID=your-client-id \
  -e EMAIL_CLIENT_SECRET=your-secret \
  -e EMAIL_USER_ID_WITH_LICENSE=sender@domain.com \
  --name ms-emails-rest \
  ms-emails-rest:latest
```

**gRPC**:
```bash
docker build -t ms-emails-grpc:latest \
  -f src/entrypoints/CodeDesignPlus.Net.Microservice.Emails.gRpc/Dockerfile .

docker run -d -p 5001:5001 \
  --network=backend \
  -e ASPNETCORE_ENVIRONMENT=Docker \
  --name ms-emails-grpc \
  ms-emails-grpc:latest
```

**AsyncWorker**:
```bash
docker build -t ms-emails-worker:latest \
  -f src/entrypoints/CodeDesignPlus.Net.Microservice.Emails.AsyncWorker/Dockerfile .

docker run -d \
  --network=backend \
  -e ASPNETCORE_ENVIRONMENT=Docker \
  --name ms-emails-worker \
  ms-emails-worker:latest
```

### Kubernetes Deployment

Helm charts available in `charts/` directory:

```bash
# Deploy REST API
helm install ms-emails-rest ./charts/ms-emails-rest \
  --set image.tag=latest \
  --set email.tenantId=your-tenant-id \
  --set email.clientId=your-client-id \
  --set-file email.clientSecret=./secrets/client-secret.txt

# Deploy gRPC
helm install ms-emails-grpc ./charts/ms-emails-grpc \
  --set image.tag=latest

# Deploy Worker
helm install ms-emails-worker ./charts/ms-emails-worker \
  --set image.tag=latest
```

### Health Checks

```bash
# REST API health endpoint
curl http://localhost:5000/health

# Kubernetes liveness/readiness probes
livenessProbe:
  httpGet:
    path: /health
    port: 5000
  initialDelaySeconds: 30
  periodSeconds: 10
```

## 📚 Additional Documentation

- **API Reference**: `/swagger` endpoint (REST API)
- **gRPC Definitions**: `src/entrypoints/CodeDesignPlus.Net.Microservice.Emails.gRpc/Protos/emails.proto`
- **CodeDesignPlus SDK**: [Documentation](https://codedesignplus.github.io/)
- **Microsoft Graph API**: [Docs](https://learn.microsoft.com/en-us/graph/api/user-sendmail)

## 🛠️ Development Tools

### Update Packages

```bash
cd tools/update-packages
./update-packages.sh
```

### Upgrade .NET Version

```bash
cd tools/upgrade-dotnet
./upgrade-dotnet.sh
```

### SonarQube Analysis

```bash
cd tools/sonarqube
# Update SonarQube URL and token in sonarqube.sh
./sonarqube.sh
```

### Convert Line Endings

```bash
cd tools
./convert-crlf-to-lf.sh
```

## 🤝 Contributing

Please read our Contributing Guide for details on our code of conduct and development process.

### Pull Request Process

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request

### Coding Standards

- Follow Clean Code principles
- Write unit tests (80% coverage minimum)
- Use meaningful variable/method names
- Document public APIs with XML comments
- Follow SOLID principles

## 📄 License

This project is licensed under the GNU Lesser General Public License v3.0 - see the [LICENSE.md](LICENSE.md) file for details.

## 📞 Contact

**CodeDesignPlus**  
Email: wliscano@codedesignplus.com  
Website: [https://codedesignplus.com](https://codedesignplus.com)

## 🙏 Acknowledgments

- **CodeDesignPlus.Net.Sdk** - Core SDK and libraries
- **Microsoft Graph** - Email delivery infrastructure
- **.NET Team** - For the amazing .NET 9 framework
- **Community** - For feedback and contributions

---

**Built with ❤️ by CodeDesignPlus**
