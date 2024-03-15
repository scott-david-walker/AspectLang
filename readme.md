# Aspect

This is a prototype programing language that will be focused around API testing.

I wanted to create this as I felt the tools that were out there for API testing and testing in general are usually
either GUI based (postman etc) which doesn't work well with source control or frameworks that are attached to an existing
language as a bit of an add on. There were a few outliers who didn't fall into this but I became quite frustrated with poor
error messages etc.

So the goals for this language are:
1. A test first approach. Testing is baked into the language and should be able to be achieved without additional libraries.
2. Easy enough for someone who is not a software engineer to pick up (E.G. testers);
3. Provide useful feedback on errors 

# How the language will look (simple)

## Variables

Variables are set using the `val` keyword;

```
    val x = 5;
```
Variables are statically typed and can't be assigned a new type;

```
val x = 5; // set as int
x = "string"; // error
```

Variable types can be defined when we want to be specific.

```
val x = 5 : float; // Normally this would be an int. But specifying as a float changes it's value to 5.0; 
```

## Types

## Arrays

## Functions

## Packages

## Loops

There are two loop styles in Aspect, one for iterating for x amount of times (a for/while loop) and one for iterating over arrays.

There are multiple loop specific keywords in place to help us keep this simple yet easy to read;

| Keyword  | Description                                                                                                 |
|----------|-------------------------------------------------------------------------------------------------------------|
| continue | Will skip the rest of the loop to be executed and move onto the next iteration.                             |
| break    | Will break out of the current loop                                                                          |
| index    | References the current loop iteration. Starting at 0 and increasing with every iteration.                   |
| it       | Is used specifically when looping over an array. It takes the value of the element currently being iterated |

// Iterating over arrays
```
val x = [1,2,3];

iterate over x {
    print(it);
}
// prints 123
```

// Iterate until a condition is met
```
val x = 0;

iterate until x == 5 {
    print(x);
    x = x + 1;
}
// prints 01234
```


// Iterate until the index of the loop is 5;
```
iterate until 5 {
    print(index);
}
// prints 01234

```


### C# Interop

### Testing

### No side effects



