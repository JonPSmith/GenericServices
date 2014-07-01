GenericServices
===============

Generic Services is a .NET class library to help build a [service layer](http://martinfowler.com/eaaCatalog/serviceLayer.html), i.e. a layer that acts as a facard/adapter between your business/data service layers
and your User Interface or HTTP service. It makes heavy use of [Entity Framework 6](http://msdn.microsoft.com/en-us/data/ee712907) and .NET 4.5's (async/await)[http://msdn.microsoft.com/en-gb/library/hh191443.aspx]

### What is the motivation behind building GenericServices?
I have just finished version 1 of a fairly complex MVC web application for analysing, modelling and visualising geospatial problems. 
This combined a complex, Domain-Driven Design for the data and business objects with a powerful user interface using a [Single Page Application - SPA] (http://en.wikipedia.org/wiki/Single-page_application).
There was mismatch between what the business/data layers looked like and what the SPA wanted.

The Service Layer turned out to be one of the most important layers in that it allowed a Domain-Driven Design focus on the backend problem
while making sure that the SPA, via the WebApi, and other MVC pages had the data they needed. It used lots of Data Transfer Objects - DTOs
[See this useful article](http://msdn.microsoft.com/en-us/magazine/ee236638.aspx) to shape and enhance the data as it moved up and down the layers.

The service layer was great, but it had lots of code that was very similar with just the data being different. It was also boring to write! 
I therefore researched a number of approaches to solve this and finally came up with a solution using C#'s Generic classes.

### Generic Services contains the following basic services:

1. Database commands (both normal and Async versions)
  - Create
  - Update
  - Delete
  - Detail
  - List
2. Method commands
  - Run Method
  - Run method that writes to database.
