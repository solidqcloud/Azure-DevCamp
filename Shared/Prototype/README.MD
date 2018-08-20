# Source
# City Power & Light Prototype

## Overview
This is a "flat" HTML/CSS/JS functional prototype for the City Power & Light demonstrations.  It is intended to be a bare bones implementation of the application to vet the visual design and interaction model of the application. 

The application will be used by other projects in this repository, implementing the code into a variety of server side technologies. 

## Dependencies
While this prototype purposely does not include any specific server side specific implementation components, there are a few dependencies to note. 

* [jQuery](https://jquery.com/) - v2.2.4 is used as a cross browser JavaScript interaction library, and as a requirement for Bootstrap.  The library is being served from the [Microsoft AJAX Content Delivery Network](https://www.asp.net/ajax/cdn).

* [Bootstrap](https://getbootstrap.com/) - v3.3.6 is used as the front end component library. Its CSS and JavaScript serve as the foundation of the visual design.  The library is being served from the [Microsoft AJAX Content Delivery Network](https://www.asp.net/ajax/cdn).

* [Flatly Theme](http://bootswatch.com/flatly/) - the Flatly theme from [Bootswatch](http://bootswatch.com/) is being used for added visual appeal.  The theme is MIT Licensed and loaded via maxcdn 

`index.html` - Homepage 

`dashboard/dashboard.html` - Dashboard file showing all items

`new/new.html` - New item form

`css/main.css` - Custom Cascading Style Sheet (CSS) for adjusting visual appearance of application