GenericServices
===============

Generic Services is a .NET class library to help build a [service layer](http://martinfowler.com/eaaCatalog/serviceLayer.html), i.e. a layer that acts as a facard/adapter between your business/data service layers
and your User Interface or HTTP service. It makes heavy use of [Entity Framework 6 - EF6](http://msdn.microsoft.com/en-us/data/ee712907) 
and .NET 4.5's [async/await](http://msdn.microsoft.com/en-gb/library/hh191443.aspx)

### What is the motivation behind building GenericServices?
I finished version 1 of a fairly complex MVC web application for analysing, modelling and visualising geospatial problems in mid 2014.
This application, called [Spatial Modeller™](http://selectiveanalytics.com/about-us/spatial-modeller/) 
which combines a complex, Domain-Driven Design for the data and business objects with a powerful user interface using 
a [Single Page Application - SPA] (http://en.wikipedia.org/wiki/Single-page_application).
There was mismatch between what the business/data layers looked like and what the SPA wanted, which the service layer solved.

The Service Layer turned out to be a really important layer as it allowed a Domain-Driven Design focus on the backend problem
while providing the SPA, via the WebApi, and other MVC pages with the data in the format they needed. It used lots of Data Transfer Objects - DTOs
[See this useful article](http://msdn.microsoft.com/en-us/magazine/ee236638.aspx) to shape, alter and enhance the data as it moved up and down the layers.

The service layer was great, but it had lots of code that was very similar with just the data being different. It was also boring to write! 
I therefore researched a number of approaches to solve this and finally came up with a solution using C#'s Generic classes.

### What does the GenericService provide?

#### 1. Generic Database access command

GenericServices provides the standard CRUD (Create, Read, Update and Delete) commands using EF 6. These commands have the following features
- Can work directly on the class connected to the database OR via a DTO to shape or change the data format.
- It has pre-written CRUD commands using EF6 so no duplication of code.
- Does automatic, convention-based mapping between data classes and DTO
- For items that can't map, like needing to load a list of options for a user, there are clear extension points
- GenericService handles errors in the business or data layer and communicates them up to the UI in a clean way.
- There are normal and async versions of all CRUD commands
           
#### 2. Generic calling of business logic

GenericServices has standard patterns for short and long-running business methods
