﻿
namespace ProtoBuf.Property
{
    internal sealed class PropertyInt32Variant<TSource> : Property<TSource, int>
    {
        public override string DefinedType
        {
            get { return ProtoFormat.INT32; }
        }

        public override System.Collections.Generic.IEnumerable<Property<TSource>> GetCompatibleReaders()
        {
            yield return CreateAlternative<PropertyInt32Fixed<TSource>>(DataFormat.FixedSize);
        }
        public override WireType WireType { get { return WireType.Variant; } }

        public override int Serialize(TSource source, SerializationContext context)
        {
            int value = GetValue(source);
            if (IsOptional && value == DefaultValue) return 0;
            return WritePrefix(context)
                + context.EncodeInt32(value);
        }

        public override int DeserializeImpl(TSource source, SerializationContext context)
        {
            return context.DecodeInt32();
        }
    }
}
