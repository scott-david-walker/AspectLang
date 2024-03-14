namespace AspectLang.Parser.Compiler.ReturnableObjects;

public class ArrayReturnableObject : IReturnableObject, IIsEqual
{
    public List<IReturnableObject> Elements { get; set; }
    public bool IsEqual(IReturnableObject returnableObject)
    {
        if (returnableObject is ArrayReturnableObject arrayReturnableObject)
        {
            if (Elements.Count != arrayReturnableObject.Elements.Count)
            {
                return false;
            }
            
            for (var i = 0; i < Elements.Count; i++)
            {
                var elementOne = Elements[i] as IIsEqual;
                var elementTwo = arrayReturnableObject.Elements[i];
                if (elementOne == null)
                {
                    throw new($"Cannot compare array element with {elementTwo.GetType()}");
                }

                if (!elementOne.IsEqual(elementTwo))
                {
                    return false;
                }
            }
            return true;
        }
        throw new($"Cannot Compare integer with {returnableObject.GetType()}");    
    }
}