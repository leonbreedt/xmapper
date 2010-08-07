ObjectGraph
===========

Just a toy framework, to play with some ideas I have for object graph serialization...

* In memory, use POCO all the way for the objects,
  as that's as light as it's going to get.

* When we need to flush the object graph to disk, have two options:

	- Speedy Google Protocol Buffer serialization, for use in production,
	  when we need performance.

	- Slow-ass XML serialization (using hand-crafted XLinq generation though,
	  *NOT* XmlSerializer), for when we need to debug.

* Also, our POCO types should be equatable easily without having to implement the same
  ugly Equals()/GetHashCode() boilerplate. But equatability shouldn't come at too much 
  of a cost, so use lambda => expression tree magic to reference the equatable 
  fields or properties from our POCO types.


NOTE: Since protobuf does not preserve object references within the message
      (it's a tree format), we'll use an ID scheme for this. An object wanting to 
      take a reference to another object stores the identifier of that object 
      (string, integer), and we'll have an in-memory index that can be queried 
      to retrieve the reference once the graph is loaded.