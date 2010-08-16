ObjectGraph
===========

Just a toy framework, to play with some ideas I have for object graph serialization...

No guarantees that it compiles, or even, works.

* In memory, pretty plain C# objects, annotated with DataContract/DataMember. These
  attributes are used for both protobuf-net and XML serialization. 

* Seperate class for serialization/deserialization, keepin' it POCO.

* When we need to flush the object graph to disk, have two options:

    - Speedy Google Protocol Buffer serialization, for use in production,
      when we need performance.

    - Slow-ass XML serialization (using hand-crafted XmlWriter generation though,
      *NOT* XmlSerializer), for when we need to debug.

NOTE: Since protobuf does not preserve object references within the message
      (it's a tree format), we'll have some mechanism by which objects with
      identifiers can be registered with an index on deserialization. When 
      we have objects needing "references" to other objects, we just store
      the ID, and get the reference by an index query.