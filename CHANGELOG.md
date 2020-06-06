# Changelog

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.2.1] - 2020-06-06
### Added
- DropdownValueDatabindingBuilder to enable binding the selected index of a Dropdown control to an int in the ViewModel.

### Changed
- Improved the TwoWayBinding example to include a more elegant and less intrusive way (keeping the view model clean and simple) of feeding input back into the application.

### Fixed
- Fixed a bug where Tools -> Find Unbound BindingBuilders would report false positives.


## [1.1.1] - 2020-05-29
### Added
- Tool Menu to find unbound property binding builders.
- Background of unbound properties now shows red to indicate unbound binding builders


## [1.0.1] - 2020-05-22

### Removed not needed .meta files.

## [1.0.0] - 2020-05-20

### This is the first release of *Unity DataBinding*.