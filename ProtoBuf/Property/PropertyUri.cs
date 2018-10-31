﻿using System;
using System.Text;

namespace ProtoBuf.Property
{
    internal sealed class PropertyUri<TSource> : Property<TSource, Uri>
    {
        public override string DefinedType { get { return ProtoFormat.STRING; } }
        public override WireType WireType { get { return WireType.String; } }

        private Property<string, string> innerSerializer;
        protected override void OnBeforeInit(int tag, ref DataFormat format)
        {
            innerSerializer = PropertyFactory.CreatePassThru<string>(tag, ref format);
            base.OnBeforeInit(tag, ref format);
        }
        public override int Serialize(TSource source, SerializationContext context)
        {
            Uri value = GetValue(source);
            if (value == null || (IsOptional && value == DefaultValue)) return 0;
            return innerSerializer.Serialize(value.AbsoluteUri, context);
        }

        public override Uri DeserializeImpl(TSource source, SerializationContext context)
        {
            string value = innerSerializer.DeserializeImpl(null, context);
            return string.IsNullOrEmpty(value) ? null : new Uri(value);
        }
    }
}
