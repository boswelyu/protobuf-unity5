﻿
namespace ProtoBuf.Property
{
    internal sealed class PropertyChar<TSource> : Property<TSource, char>
    {
        public override string DefinedType
        {
            get { return ProtoFormat.INT32; }
        }
        public override WireType WireType { get { return WireType.Variant; } }

        public override int Serialize(TSource source, SerializationContext context)
        {
            char value = GetValue(source);
            if (IsOptional && value == DefaultValue) return 0;
            return WritePrefix(context)
                + context.EncodeUInt32(value);
        }

        public override char DeserializeImpl(TSource source, SerializationContext context)
        {
            return (char) context.DecodeInt32();
        }
    }
}

