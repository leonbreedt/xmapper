ObjectGraph
===========

Just a toy framework, to play with some ideas I have for object graph serialization...

No guarantees that it compiles, or even, works.

Requirements
------------

* Plain old C# objects as nodes in the graph, each type having no knowledge of how it is
  serialized.

* One serializer class that can serialize to the desired format, the formats supported being
  Protocol Buffers and XML.

* Performance, performance, performance. The reason for this framework existing is that I
  have existing issues in production with the slowness of Microsoft serialization options in
  .NET (DataContractSerializer/XmlSerializer/BinaryFormatter).


Protocol Buffer Serialization
=============================

I re-use the excellent protobuf-net (http://code.google.com/p/protobuf-net), as it already
supports serialization of C# types annotated exactly as I want.


XML Serialization
=================

Microsoft's options in this area are staggeringly slow, I suspect a lot of reflection is used,
and they have constraints of having to support every permutation customers may ask for.

So, we hand-craft yet another XML serialization framework, this one primarily convention based
and making conscious decisions not to support XML features that we will not need or use.

Why not JSON? Well, not ruling it out, if XmlWriter/XmlReader turns out still to be too slow :)


Serializer Design
=================

Taking a leaf from the excellent work done by Jon Skeet and Marc Gravell in finding a way to
make reflection blisteringly fast...

We use generics to pass the type argument to our serializer class, and then use the feature
of C# where static members are bound to each distinct closed generic class to pre-cache the delegates
that will be called to read or write data.

We never make a reflection call during serialization. 

To read an XML value, we call a pre-cached delegate.
To write an XML value, we call a pre-cached delegate.
To set an object property, we call a pre-cached delegate, passing through the object instance.
To read an object property, we call a pre-cached delegate, passing through the object instance.

If it turns out that we are prevented from supporting certain features due to this aggressive caching,
that's too bad.

Performance is not negotiable.


Example 1
---------

This is an example of a type that contains only simple types.

Simple types being types defined in the TypeCode enumeration 
that do not resolve to Object, DBNull, or Empty.

    [DataContract]
    class Person
    {
        [DataMember] public string FirstName { get; set; }
        [DataMember] public string LastName { get; set; }
        [DataMember] public bool IsFriend { get; set; }
    }

It serializes to:

    <Person FirstName='Name1' LastName='Name2' IsFriend='true' />


Example 2
---------

This is an example of a type that contains simple types as well as user
defined types.

    [DataContract]
    class Person
    {
        [DataMember] public string FirstName { get; set; }
        [DataMember] public string LastName { get; set; }
        [DataMember] public Address HomeAddress { get; set; }
    }

    [DataContract]
    class Address
    {
        [DataMember] public string StreetNumber { get; set; }
        [DataMember] public string StreetName { get; set; }
    }

It serializes to:

    <Person FirstName='Name1' LastName='Name2'>
        <HomeAddress StreetNumber='42' StreetName='Queen St.' />
    </Person>


Example 3 (Collection Style 1):
-------------------------------

This is an example of a type that contains a collection. 
We do not support collections as roots, as this is not supported 
by Protocol Buffers, and doesn't really make sense (you can't have
a collection as root of an XML document either).

A type that contains a collection can still contain everything listed before.

By default, the collection will serialize to child elements directly contained
by the parent.

    [DataContract]
    class Person
    {
        [DataMember] public List<PhoneNumber> PhoneNumbers { get; set; }
    }

    enum PhoneType
    {
        Mobile,
        Home,
        Work
    }

    [DataContract]
    class PhoneNumber
    {
        [DataMember] public PhoneType Type { get; set; }
        [DataMember] public string Number { get; set; }
    }

It serializes to:

    <Person>
        <PhoneNumber Type='Mobile' Number='021-123-456' />
        <PhoneNumber Type='Work' Number='09-555-1234' />
    </Person>

Example 3 (Collection Style 2):
-------------------------------

This is an example of a type that inherits from List<T>, used
when we want the items in the collection to wrapped by a container
element.

    [DataContract]
    class Person
    {
        [DataMember] public PhoneNumbers PhoneNumbers { get; set; }
    }

    enum PhoneType
    {
        Mobile,
        Home,
        Work
    }

    [CollectionDataContract]
    class PhoneNumbers : List<PhoneNumber>
    {
    }

    [DataContract]
    class PhoneNumber
    {
        [DataMember] public PhoneType Type { get; set; }
        [DataMember] public string Number { get; set; }
    }

It serializes to:

    <Person>
        <PhoneNumbers>
            <PhoneNumber Type='Mobile' Number='021-123-456' />
            <PhoneNumber Type='Work' Number='09-555-1234' />
        </PhoneNumbers>
    </Person>

Limitations
===========

We only serialize types annotated with [DataContract], or simple types. The root type
must be a data contract, and is not allowed itself to be a collection (though it may contain
collections).

We only serialize public object properties annotated with [DataMember].


Since Protocol Buffers does not preserve object references within the message
we'll have some mechanism by which objects with identifiers can be registered 
with an index on deserialization. 

When we have objects referencing other objects, we just store the identifier,
and look it up in the index when we need it.