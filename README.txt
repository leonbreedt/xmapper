ObjectGraph
===========

Just a toy framework, to play with some ideas I have for object graph serialization...

No guarantees that it compiles, or even, works.

* In memory, pretty plain C# objects, annotated with DataContract/DataMember so protobuf-net
  works. Using List<T> for collections.

* Seperate class for serialization/deserialization.

* When we need to flush the object graph to disk, have two options:

    - Speedy Google Protocol Buffer serialization, for use in production,
      when we need performance.

    - Slow-ass XML serialization (using hand-crafted XLinq generation though,
      *NOT* XmlSerializer), for when we need to debug.

* Have the notion of an object index, within which objects with identifiers
  can index themselves via deserialization callbacks.

NOTE: Since protobuf does not preserve object references within the message
      (it's a tree format), we'll use an ID scheme for this. An object wanting to 
      take a reference to another object stores the identifier of that object 
      (string, integer), and we'll have an in-memory index that can be queried 
      to retrieve the reference once the graph is loaded.