# MergeO
Merge Objects

## Description
This library takes two objects of the same type and performs a merge based on the supplied values
within the object graph. Each merge is customizable and the merge behaviour can be overridden.

The primary use cases for this library are when you have a current object state and you need to
merge in partial new data (e.g., if the user submitted new details or updates but didn't have to
fill out the full details).

## Examples
Let's say that we have a simple class like so ([sample code](https://github.com/DiskJunky/MergeO/tree/master/MergeO.Samples/Simple));
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

However, later our user wanted to update just their name in our versioned database and only the
`FirstName` was updated. The model supplied from the UI now looks like this;
```
ID = 0
FirstName = "Joseph"
LastName = null
```

We want to merge these two models so that only `FirstName` is updated. Using MergeO, we can
create a default merger for our objects like so;
```
using MergeO.Contracts;

class PersonMerger
{
    public Person Merge(Person first, Person second)
    {
        // merge pattern defaults to NeverOverwriteOldWithNull()
        IMerger merger = new Merger();

        return _merger.MergeItems(first, second);
    }
}
```

The resulting merged model will now look like;
```
ID = 1
FirstName = "Joseph"
LastName = "Soap"
```

## Complex Examples
MergeO can deal with more complex merges, handing nested objects, collections, custom merge
styles and custom merge styles only at particular points in the model. This section will
outline each of these scenarios.

### Nested Merges
For nested objects, the same principles apply. The merge strategy (more on that later), used
to apply to the root object will also apply to any child objects. E.g., our `Person` class now
has a property called `Pet` that looks like;
```
class Pet
{
    public string Name { get; set; }
    public DateTime? LastFed { get; set; }
}
```

If the original data looks like;
```
ID = 1
FirstName = "Joseph"
LastName = "Soap"
Pet = {
    Name = "Rintintin"
    LastFed = null
}
```

And we update with the latest feeding date/time such that object instance two looks like;
```
ID = 0
FirstName = null
LastName = null
Pet = {
    Name = null
    LastFed = "2019-01-01 13:00"
}
```

Then the merged model for these two instances will return a merged object graph that looks
like;
```
ID = 1
FirstName = "Joseph"
LastName = "Soap"
Pet = {
    Name = "Rintintin"
    LastFed = "2019-01-01 13:00"
}
```

Unit tests found at `MergeO.UnitTests.MergerTests.Merger_MergesNestedObjectValues()` [here](https://github.com/DiskJunky/MergeO/blob/7daf5fe158a511e63ebb33689ef817f9a557f605/MergeO.UnitTests/MergerTests.cs#L105).

### Collections
Collections get compilcated as there are a variety of ways of merging them. The default behaviour is to
replace the original collection if a newer one is not null (this follows the default merge strategy of
`NeverOverwriteOldWithNull` [here](https://github.com/DiskJunky/MergeO/blob/master/MergeO/MergeCriteria/NeverOverwriteOldWithNull.cs),
though this can be overridden). This is of limited value however, as it may be desired that the
collection merge in some predefined manner if two object graphs contain them.

For example, `InterpolateLists` ([here](https://github.com/DiskJunky/MergeO/blob/master/MergeO/MergeCriteria/InterpolateLists.cs))
will take the two collections, append the second to the first and then sort them as per the configured
`IComparer<T>` implementation. However, it is up to you to write the `ÌComparer<T>` implementation
as there is no way for the library to know ahead of time how it should sort a given object.

What does this look like in practice? Let's say that our `Pet` property gets expanded to `List<Pet>`
and we want to sort by `Name` as pets are added to the details. We'd have to create a `PetComparer`
class like so that (for the purposes of this example), simply wraps `string.Compare()`;
```
class PetComparer : IComparer<Pet>
{
    public int Compare(Pet x, Pet y)
    {
        return string.Compare(x?.Name, y?.Name);
    }
}
```

That creates our comparer, but how do we tell the merge to use it? To do this, we need to pass in a
collection of customized `IMergeCriteria` that tell the merger how it should perform the merge using
our new `PetComparer`. Following on from the simple example earlier where we had;
```
public Person Merge(Person first, Person second)
{
    // merge pattern defaults to NeverOverwriteOldWithNull()
    IMerger merger = new Merger();

    return _merger.MergeItems(first, second);
}
```

We'll want to take a few extra steps. First, we need to tell the `Merger` instance where to apply
the custom rule. We do this by using `BreadcrumbHelper`. This creates a keyed string that the 
merger uses to identify where in the object graph it is as it traverses. We want our rule to apply
at the `Person.Pets` property like so;
```
var key = BreadcrumbHelper<Person>.Of(p => p.Pets);
```

Now we'll want to specify the rule with our key and comparer;
```
var petMerger = new InterpolateLists(key,
                                     new PetComparer());
var mergeCriteria = new List<IMergeCriteria>();
mergeCriteria.Add(petMerger);
```

All together our `Merge()` method now looks like;
```
public Person Merge(Person first, Person second)
{
    var key = BreadcrumbHelper<Person>.Of(p => p.Pets);
    var petMerger = new InterpolateLists(key,
                                         new PetComparer());
    var mergeCriteria = new List<IMergeCriteria>();
    mergeCriteria.Add(petMerger);

    // merge pattern defaults to NeverOverwriteOldWithNull()
    IMerger merger = new Merger(mergeCriteria);

    return _merger.MergeItems(first, second);
}
```

This is fine for one or two custom rules but would quickly become cumbersome for large objects or a
large number of customizations. For these scenarios, it makes more sense to created a dedicate custom
merger that wraps the rules into a single location.

Other custimizations can be done taking the same approach as seen here. You could specify
`AlwaysUseNewer` at a particular location using the same approach though without needing to create
a customer comparer.

Unit tests found at `MergeO.UnitTests.MergerTests.Merger_DefaultMergeCriteria_UsesNewValues(params ComplexObjectNullableFields[] history)` [here](https://github.com/DiskJunky/MergeO/blob/7daf5fe158a511e63ebb33689ef817f9a557f605/MergeO.UnitTests/MergerTests.cs#L53).

### Creating custom `IMergerCriteria`
TODO