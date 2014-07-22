GenericServices
===============

Generic Services is a .NET class library to help build a [service layer](http://martinfowler.com/eaaCatalog/serviceLayer.html), i.e. a layer that acts as a facard/adapter between your business/data service layers and your User Interface or HTTP service. It makes heavy use of [Entity Framework 6 - EF6](http://msdn.microsoft.com/en-us/data/ee712907) and .NET 4.5's [async/await](http://msdn.microsoft.com/en-gb/library/hh191443.aspx) commands. Its aim is to make the creation of the service layer simple while providing robust implementations of standard database and business object actions. It is an Open Source project.

#### Go to the [live example web site](http://samplemvcwebapp.net/) for live demostrations of GenericServices working in an ASP.NET MVC5 application.


### What is the motivation behind building GenericServices? 

I develop fairly complex analysing, modelling and data visualisation web applications (see [Spatial Modeller](http://selectiveanalytics.com/about-us/spatial-modeller/)). These require a Domain-Driven Design approach to the data and business objects, while the visualisation needs a comprehensive user interface which I implement using a [Single Page Application - SPA](http://en.wikipedia.org/wiki/Single-page_application). This means there often a mismatch between what the business/data layers classes and what the user interface needs.

My experience is that the Service Layer, plus [Data Transfer Objects - DTOs](http://msdn.microsoft.com/en-us/magazine/ee236638.aspx) is the best way to solve this mismatch. However I have found that the service layer is often filled with lots of code that is very similar, with just different data classes. I therefore researched a number of approaches to solve this and finally came up with a solution using C#'s Generic classes. I have therefore called it GenericServices.

### What does the GenericServices framework provide?

#### 1. Generic Database access commands

GenericServices provides the standard CRUD (Create, Read, Update and Delete) commands using EF 6. These commands have the following features:

- Standard CRUD commands using EF6 that can link to any class through C# Generics.
- Can work directly on the class connected to the database OR via a DTO to shape the data.
- Does automatic, convention-based mapping between data and DTO classes via [AutoMapper](https://github.com/AutoMapper/AutoMapper/wiki).
- Good extension points to handle specific issues like loading a dropdownlist for the UI.
- There are normal and async versions of all CRUD commands.

#### 2. Generic calling of business logic

GenericServices has standard patterns for running business methods. The features are:

- Ability to call a business method normally or as async task.
- Ability to copy DTO to business data class (same methods as for database commands).
- External code available for handling long-running methods with progress and cancel.

#### 3. What frameworks are GenericServices is designed to work with?

- GenericServices is designed work as a service layer framework in any .NET application, such as: 
  - [ASP.NET MVC](http://www.asp.net/mvc/tutorials/mvc-5/introduction/getting-started)
  - [Widows Azure Web apps](https://azure.microsoft.com/en-us/services/web-sites/)
  - etc. 
- GenericServices assumed a disconnected state model, e.g. a web site or a Http RESTful service .
- GenericServices assumes a horizontal scaling model, e.g. scale by having multiple web instances. 
- GenericServices uses the following .NET frameworks/systems.

  - It needs .NET 4.5 for the new [async/await](http://msdn.microsoft.com/en-gb/library/hh191443.aspx) tasking format introduced in .NET 4.5
  - It uses [Entity Framework 6](http://msdn.microsoft.com/en-us/data/ee712907) for database access, again because it supports async commands.
  - It also makes use of the open source [AutoMapper](http://automapper.org/) library for transforming data to/from DTOs.

#### 4. General items

- GenericService is designed to handle validation and error checking at multiple levels in the system.
- Good examples of usage via an online [example web site](http://samplemvcwebapp.net/) which includes some documentation.
- The source code of the [example web site](https://github.com/JonPSmith/SampleMvcWebApp) is also available as a Open Source project.
- The commands have been extensively Unit Tested.
- The project is Open Source.


### List of commands
