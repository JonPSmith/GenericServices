GenericServices
===============

GenericServices is a .NET class library to help build a 
[service layer](http://martinfowler.com/eaaCatalog/serviceLayer.html), 
i.e. a layer that acts as a facard/adapter between your business/data service layers 
and your User Interface or HTTP service. It makes heavy use of 
[Entity Framework 6 - EF6](http://msdn.microsoft.com/en-us/data/ee712907) 
and .NET 4.5's [async/await](http://msdn.microsoft.com/en-gb/library/hh191443.aspx) 
commands. Its aim is to make the creation of the service layer simple while 
providing robust implementations of standard database access commands. 
GenericServices is an Open Source project under the 
[MIT licence](https://github.com/JonPSmith/GenericServices/blob/master/licence.txt)

## The best documentation is a demo
I have created two demo sites that show how GenericServices can be used to produce a
ASP.NET MVC web application with a html/razor front end. 

#### 1. [SampleMvcWebApp](http://samplemvcwebapp.net/) web site
This is an introductory/basic web site using GenericServices based on an database
containing blog post.
It is an open-source project with 
[source code](https://github.com/JonPSmith/SampleMvcWebApp) available.

Read the [site's introduction](http://samplemvcwebapp.net/) and the 
[source code README page](https://github.com/JonPSmith/SampleMvcWebApp) for more information.

#### 2. [SampleMvcWebAppComplex](https://github.com/JonPSmith/SampleMvcWebAppComplex) web site
This is an Advanced/Complex web site using GenericServices to provide a user interface
to the AdventureWorks Lite 2012 database. This is NOT an open-source project,
mainly because it uses the proprietary, paid-for library 
[Kendo UI MVC](http://www.telerik.com/kendo-ui). 
However the source is made available as it is a good reference source.

Read the [site's introduction](http://complex.samplemvcwebapp.net/) and the 
[source code README page](https://github.com/JonPSmith/SampleMvcWebAppComplex) 
for more information.

## Second best documentation is the Wiki
The [GenericServices Wiki](https://github.com/JonPSmith/GenericServices/wiki)
has extensive information - see the sidebar for an index to the various parts.

The wiki documentation covers things the software doesn't explain, like
[Why GenericServices?](https://github.com/JonPSmith/GenericServices/wiki/Why-GenericServices%3F) 
and [Architecture Overview](https://github.com/JonPSmith/GenericServices/wiki/Architecture-Overview).
Well worth a look.

## Other information

I have written a number of articles for the 
[Simple-Talk web site](https://www.simple-talk.com/author/jon-smith/),
all of which are relevant to GenericServices as the code in all four
articles came from the two demo sites.




