# Changelog

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2022-12-01

### Removes
- The formerly deprecated binder workflow has been removed.
  
### Adds
- A new feature to quickly access the source code of a data source was added.
  
  ![Access source code of DataSource](Documentation~/Images/AccessSourceCodeOfDataSource.png)

- Incomlete bindings now show a hint in the user interface.

### Changes
- The editors user interface has been rewritten using UI Toolkit.
- Consistency checks are now integrated into the Inspector UI.



## [1.3.1] - 2021-11-13

### Adds 
 - UIDocumentView : DataBinding extended to VisualElements and VisualElement Styles.
 - A new Example demonstrating how to use the new features.

## [1.3.0] - 2021-11-08

### Adds

- View MonoBehaviour as an "all in one" approach for GameObject targeted DataBinding. View combines setting up property and container bindings in a single user interface element.
  
  [[/Documentation~/Images/ViewEditor.png|View Inspector]]

  Views can bind Properties of DataSources to Properties of Unity Components.
  Container Bindings can be used to generate dynamic content using a Prefabs as template for items of the collection.

  Views are intentionally introduced in parallel to the existing approach to enable a fluent transformation to a UIToolkit based implementation.

  Views are meant to replace ViewBehaviours in the long run.

  Views are the first step to extend DataBinding to support "UIToolkit at runtime" in the upcoming releases.

- UIToolkit based ViewEditor UI

## [1.2.3] - 2020-10-19

### Fixes

- Fixes namespaces of tests.
- Fixes broken display of binding builders due to introduction of condensed display option.

## [1.2.2] - 2020-08-05

### Adds

- Condensed format for validated property binding builders. Enabled by check box in ViewBehaviour.

### Fixes

- Improved performance for selecting DataSource types.

## [1.2.1] - 2020-06-06

### Adds

- DropdownValueDatabindingBuilder to enable binding the selected index of a Dropdown control to an int in the ViewModel.

### Changes

- Improved the TwoWayBinding example to include a more elegant and less intrusive way (keeping the view model clean and simple) of feeding input back into the application.

### Fixes

- Fixed a bug where Tools -> Find Unbound BindingBuilders would report false positives.

## [1.1.1] - 2020-05-29

### Adds

- Tool Menu to find unbound property binding builders.
- Background of unbound properties now shows red to indicate unbound binding builders

## [1.0.1] - 2020-05-22

### Removes not needed .meta files

## [1.0.0] - 2020-05-20

### This is the first release of *Unity DataBinding*
