﻿
namespace ProtoBuf.Property
{
    internal sealed class PropertyDouble<TSource> : Property<TSource, double>
    {
        public override string DefinedType
        {
            get { return ProtoFormat.DOUBLE; }
        }
        public override WireType WireType { get { return WireType.Fixed64; } }

        public override int Serialize(TSource source, SerializationContext context)
        {
            double value = GetValue(source);
            if (IsOptional && value == DefaultValue) return 0;
            return WritePrefix(context) + context.EncodeDouble(value);
        }

        public override double DeserializeImpl(TSource source, SerializationContext context)
        {
            return context.DecodeDouble();
        }
    }
}
