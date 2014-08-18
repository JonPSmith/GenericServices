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
#### Summary
The commands listed below work on either a class that is used in the EF database, called data class from now on, or a DTO class that inherits from EfGenericDto or EfGenericDtoAsync. Most commands can work with either a data class or a DTO

You can see examples of a ASP.NET MVC controller using these commands:
- Data class (Tag), synchonous [TagsController](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/SampleWebApp/Controllers/TagsController.cs)
- Data class (Tag), async [TagsAsyncController](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/SampleWebApp/Controllers/TagsAsyncController.cs)
- DTO (DetailPostDto), sync [PostsController](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/SampleWebApp/Controllers/PostsController.cs)
- DTO (DetailedPostDtoAsync), async [PostsAsyncController](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/SampleWebApp/Controllers/PostsAsyncController.cs)

#### Full list of commands

##### ListService: 
This returns an IQueryable list of the type T. Where T is either data class or DTO. 
If T is the data class then it simply returns that. 
If T is a DTO inherited from either EfGenericDto (sync) or EfGenericDtoAsync (async)
then the data properties are copied over using a convention-based object-object mapping 
(see [AutoMapper](http://automapper.org/)). This allows the data to be 'shaped'. 
See [SimplePostDto](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/ServiceLayer/PostServices/Concrete/SimplePostDto.cs)
for an example of this.

Note that the ListService is not sync or async, but returns `IQueryable`. 
The LINQ command you put on the end, e.g `.ToList()` or `.ToListAsync()`, 
determines whether it is async or not. 

The command is:

- `IQueryable<T> GetList<T>()`


##### DetailService/DetailServiceAsync : 
This finds an item in the database using its primary key(s). You specify what type of
class you want returned by providing the type T. 
If T is the data class then it simply returns that. 
If T is a DTO inherited from either EfGenericDto (sync) or EfGenericDtoAsync (async)
then the data properties are copied over using a convention-based object-object mapping 
(see [AutoMapper](http://automapper.org/)). This allows the data to be 'shaped'.
See [DetailPostDto](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/ServiceLayer/PostServices/Concrete/DetailPostDto.cs)
for an example of this.
The commands are:

- `T GetDetail<T>( param object [] keys)` - sync
- `Task<T> GetDetailAsync<T>( param object [] keys)` - async


##### CreateSetupService/CreateSetupServiceAsync: 
This is used to get a class to show to the user so they can 
fill in the properties etc to create a new data item. 
You define what you require by providing of type T, 
where T is either a data class or a DTO of the right type. 

If the item is a DTO then method `SetupSecondaryData` is optionally called. 
This allows the developer to overide `SetupSecondaryData` and add any other property setup
needed in the Dto. See [DetailPostDto](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/ServiceLayer/PostServices/Concrete/DetailPostDto.cs)
for an example of overriding `SetupSecondaryData`.
The commands are:

- `T GetDto<T>()` - sync
- `Task<T> GetDtoAsync<T>()` - async

##### CreateService/CreateServiceAsync
This creates a new data item from the class it is provided. 
The provided class should be either a class used in the EF database or
a DTO which must be inherited from `EfGenericDto` or `EfGenericDtoAsync` respectively.
This class must contain a property, or properties, that are the primary keys of the class 
and these keys must already been filled in (see CreateSetupService/CreateSetupServiceAsync above).

If the data T is a DTO then it calls the `CopyDtoToData` method which only copies over the 
properties in the DTO that have public setters into the original data. This gives the developer 
the option of excluding updates of certain properties if they so which. 
Also the developer can override the method `CopyDtoToData` to incorportate their own 
processes to set the right values of the properties before calling `base.CopyDtoToData`
(see example in [DetailPostDto](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/ServiceLayer/PostServices/Concrete/DetailPostDto.cs)).
The commands are:

- `ISuccessOrErrors Create( newData)` - sync
- `Task<ISuccessOrErrors> CreateAsync( newData)` - async

The `ISuccessOrErrors` class returns a status. If successful the property `.IsValid` is true and
the property `SuccessMessage` has a confirmation message. If the property `.IsValid` is false 
then the List property `Errors` contains a list of errors. 
If the status is not valid then the command calls `SetupSecondaryData`
to ensure any secondary properties needed to create the item are set 
(see CreateSetupServices above on why this is called).

One other command exists in the DTO version, called `ResetDto( dto)`. 
This should be called if there any model errors as, if required, it calls `SetupSecondaryData`
to ensure any secondary properties needed to create the item are set. 
The two versions of this command are:

- `TDto ResetDto( dto)` - sync
- `TDto ResetDtoAsync( dto)` -async

 
##### UpdateSetupService/UpdateSetupServiceAsync
This reads the data of an existing item in the database using its primary key(s). 
You specify what type of class you want returned by providing the type T.
If T is the data class then it simply returns the data found in the database.

If the item is a DTO then, like CreateSetupService, the data is copied from the 
linked data class using convention-based object-object mapping(see [AutoMapper](http://automapper.org/))
and the method `SetupSecondaryData` is optionally called. This allows the developer to
overide `SetupSecondaryData` and calculate any other properties needed in the Dto.
See an example of a DTO based UpdateSetup/Update action in
[PostsAsyncController](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/SampleWebApp/Controllers/PostsAsyncController.cs).
The commands are:

- `T GetOriginal<T>( param object [] keys)` - sync
- `Task<T> GetOriginalAsync<T>( param object [] keys)` - async

##### UpdateService/UpdateServiceAsync
This updates a data item in the database using the data handed to it. The data, which can be a data class
or DTO which inherits from EfGenericDto/EfGenericDtoAsync, must have the primary key(s) of the
item to update correctly set. 

In the case of a data item it simply replaces all the data in the original data item. 
In the case of a DTO then the method loads the original data item and then it copies over the 
properties in the DTO that have public setters into the original data. 
This gives the developer the option of excluding updates of certain properties if they so which. 
Also, just like in the CreateService, the developer can override the 
method `CopyDtoToData` to incorportate their own processes to set the right values of the properties.
(see example in [DetailPostDto](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/ServiceLayer/PostServices/Concrete/DetailPostDto.cs)).
The commands are:

- `ISuccessOrErrors Update( updatedData)` - sync
- `Task<ISuccessOrErrors> UpdateAsync( updatedDto)` - async
- `TDto ResetDto( dto)`
- `TDto ResetDtoAsync( dto)`

See CreateService/CreateServiceAsync for explanation of `ISuccessOrErrors` and `ResetDto()`. 


##### DeleteService/DeleteAsyncService
The delete service deletes a data item from the database using its primary key(s). 
The commands are:

- `ISuccessOrErrors Delete<T>( param object [] keys)` - sync
- `Task<ISuccessOrErrors> DeleteAsync<T>( param object [] keys)` - async

See CreateService/CreateServiceAsync for explanation of `ISuccessOrErrors`. 