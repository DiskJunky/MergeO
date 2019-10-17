namespace MergeO.Mocks
{
    using System.Collections.Generic;
    using Models;

    public static class NestedObjectBuilder
    {
        public static NestedObject Create()
        {
            return new NestedObject();
        }

        public static NestedObject WithByteArray(this NestedObject obj, byte[] array)
        {
            obj.Child.ByteArrayValue = array;
            return obj;
        }

        public static List<NestedObject> BuildSequence(params NestedObject[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].Child == null) continue;
                objects[i].Child.IntValue = i;
            }

            return new List<NestedObject>(objects);
        }
    }
}
