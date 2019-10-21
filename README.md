# MergeO
Merge Objects

## Description
This library takes two objects of the same type and performs a merge based on the supplied values within the object graph. Each merge is customizable and the merge behaviour can be overridden.

The primary use cases for this library are when you have a current object state and you need to merge in partial new data (e.g., if the user submitted new details or updates but didn't have to fill out the full details).

## Examples
Let's say that we have a simple class like so;
```
class Person
{
    public int ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

And in our DB, our first record has values such that;
```
ID = 1
FirstName = "Joe"
LastName = "Soap"
```

However, later our user wanted to update just their name in our versioned database and only the `FirstName` was updated. The model supplied from the UI now looks like this;
```
ID = 0
FirstName = "Joseph"
LastName = null
```

We want to merge these two models so that only `FirstName` is updated. Using MergeO, we can create a default merger for our objects like so;
```
using MergeO.Contracts;

public Person Foo(Person original, Person updatedLastName)
{
    var merger = new Merger();    // default merge style will not overwrite with null or default values
    return = merger.MergeItems(original, updatedLastName); 
}
```

The resulting merged model will now look like;
```
ID = 1
FirstName = "Joseph"
LastName = "Soap"
```

## Complex Examples
MergeO can deal with more complex merges, handing nested objects, collections, custom merge styles and custom merge styles only at particular points in the model. This section will outline each of these scenarios.

### Nested Merges
TODO; fill out. Unit tests found at `MergeO.UnitTests.MergerTests.Merger_MergesNestedObjectValues()`

### Collections
TODO; fill out. Unit tests found at `MergeO.UnitTests.MergerTests.Merger_DefaultMergeCriteria_UsesNewValues(params ComplexObjectNullableFields[] history)`

### Custom Merges
TODO; fill out. Unit tests found at `MergeO.UnitTests.MergerTests.Merger_NeverOverwriteNullMergeCriteria_DoesntOverwriteWithNulls()`

### Custom Merge at Specific Property
TODO; fill out. Unit tests found at `MergeO.UnitTests.MergerTests.Merger_NeverOverwriteNullMergeCriteria_DoesntOverwriteWithNulls()`