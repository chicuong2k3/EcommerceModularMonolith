# Ecommerce Modular Monolith

A production-ready e-commerce platform built as a Modular Monolith using .NET, designed to balance the simplicity and performance of a monolithic architecture with the modularity and scalability of microservices. The system leverages Domain-Driven Design (DDD), CQRS, and Event-Driven Architecture to deliver a robust, maintainable, and scalable solution for online retail.

## Key Features

### Catalog Module

- Manage products with variants such as size, color, and material, supporting flexible attribute configurations.
- Organize products in a tree-like structure with parent-child category relationships.
- Advanced search capabilities with filters for attributes, categories, and price ranges...

### Ordering Module

- Create, update, and manage shopping carts.
- Monitor order lifecycle with support for cancellations and returns.
- Place orders with domain events to trigger post-processing tasks.
- Ensure reliable event delivery for side effects using the Outbox pattern.

### Payment Module

- Support multiple payment gateways.
- Transaction-safe handling of payment processes.
- Manage refund requests with event-driven workflows.


