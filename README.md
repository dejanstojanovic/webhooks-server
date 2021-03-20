# Webhooks server implementation with ASP.NET Core
Simple implementation of webhooks in .NET Core using microservices

# What is it?
If you google webhooks in ASP.NET Core you are most likely going to find webhook client implementations and examples how to consume some of the popular public service webhooks (Azure, Azure DevOps, Dropbox...) but you won't find much examples on how to buld your own webhooks server.

This repository tends to tackle this problem in a unique way which allows you to apply it as a base for your own webhook server. It heavily relies on Swagger to dynamically generate the endpoint documentations for all the domain models you may want to publish.

The core idea behind it is to allow you to simply just add your domain event and it will generate the endpoints for publishing automatcally (implemented) as well as the messaging infrastructure (to be implemented). This allow it to be widely used as a base for your own webhook server.
