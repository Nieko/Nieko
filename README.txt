Nieko is a framework to support rapid development of data entry style modular applications in Microsoft .NET WPF.The framework makes heavy use of the concept of Model Views; reshaping entities and their relationships for use in the UI or in business rules.

The current version is targetted for .NET 4.0, Prism 4.1., Entity Framework 4.1. and has the following features:

- Fluent interface for rapid design of persistable, editable ViewModels. The IGraphFactory is the heart of the framework and provides just-in-time loading / saving, master-detail relationship management and persistence, Backgroud / UI thread management for working with data, and automatic mapping from entities to Model Views.

-  A Record Navigation Bar control; i.e. Data Navigation ("First", "Previous", "Last", etc) and Record editing ("New", "Cancel", "Delete", etc).

- Internal artifact navigation. Declare what artifacts (i.e. reports, users screens, etc) exist in an infrastructure library so they may be used without referencing their implementation. 

- Thoroughly decoupled persistence Units-Of-Work. The lifetime of a unit-of-work is no longer the concern of classes performing CRUD operations; creation, use and ending of unit-of-work contexts are managed in the IDataStoresManager class.