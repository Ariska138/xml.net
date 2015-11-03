# Xml.Net
A high performance xml serialization library for .NET applications

#Example Use
##Serialization
```csharp 
Person person = new Person();
person.Name = "Barack Obama";
person.Age = 54;
person.BirthDate = new DateTime(1961, 9, 4);
product.Children = new List<string> { "Natasha", "Malia" };

string xml = XmlConvert.SerializeObject(xml);
```
```xml
<Person>
  <Name>Barack Obama</Name>
  <Age>54</Age>
  <BirthDate>1961-09-04 00:00:00</BirthDate>
  <Children>
    <Element>Natasha</Element>
    <Element>Malia</Element>
  </Children>
</Person>
```

##Deserialization
```csharp
var xml = @"<Person>
  <Name>Barack Obama</Name>
  <Age>54</Age>
  <BirthDate>1961-09-04 00:00:00</BirthDate>
  <Children>
    <Element>Natasha</Element>
    <Element>Malia</Element>
  </Children>
</Person>"

Person person = XmlConvert.DeserializeObject<Person>(xml);
string name = person.name;
//Barack Obama
```
