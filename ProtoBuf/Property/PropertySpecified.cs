﻿
using System.Reflection;
using System;
namespace ProtoBuf.Property
{
    internal interface IPropertySpecified
    {
        void InitFromProperty(PropertyInfo property);
    }
    internal sealed class PropertySpecified
    {
        public static PropertyInfo GetSpecified(Type type, string name)
        {
            PropertyInfo prop = type.GetProperty(name + "Specified",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop == null || !prop.CanRead) return null;
            int tag;
            DataFormat fmt;
            MemberSerializationOptions options;
            Serializer.TryGetTag(prop, out tag, out name, out fmt, out options);
            return tag > 0 ? null : prop;
        }
    }
    internal sealed class PropertySpecified<TSource, TValue> : Property<TSource, TValue>, IPropertySpecified
    {
        private Getter<TSource, bool> getSpecified;
        private Setter<TSource, bool> setSpecified;

        
        public void InitFromProperty(PropertyInfo property)
        {
#if CF2
            getSpecified = delegate(TSource source) { return (bool)property.GetValue(source, null); };
            if (property.CanWrite)
            {
                setSpecified = delegate(TSource source, bool value) { property.SetValue(source, value, null); };
            }
#else
            getSpecified = (Getter<TSource, bool>)Delegate.CreateDelegate(typeof(Getter<TSource, bool>), null, property.GetGetMethod(true))
                                ?? delegate { throw new ProtoException("Property cannot be read: " + property.Name); };
            if (property.CanWrite)
            {
                setSpecified = (Setter<TSource, bool>)Delegate.CreateDelegate(typeof(Setter<TSource, bool>), null, property.GetSetMethod(true));
            }
#endif
        }

        public override string DefinedType
        {
            get { return innerProperty.DefinedType; }
        }
        private Property<TValue, TValue> innerProperty;

        public override System.Collections.Generic.IEnumerable<Property<TSource>> GetCompatibleReaders()
        {
            foreach (Property<TValue> alt in innerProperty.GetCompatibleReaders())
            {
                yield return CreateAlternative<PropertySpecified<TSource, TValue>>(alt.DataFormat);
            }
        }
        protected override void OnBeforeInit(int tag, ref DataFormat format)
        {
            innerProperty = PropertyFactory.CreatePassThru<TValue>(tag, ref format);
            base.OnBeforeInit(tag, ref format);
        }
        public override WireType WireType
        {
            get { return innerProperty.WireType; }
        }
        public override int Serialize(TSource source, SerializationContext context)
        {
            return getSpecified(source) ? innerProperty.Serialize(GetValue(source), context) : 0;
        }
        public override TValue DeserializeImpl(TSource source, SerializationContext context)
        {
            if (setSpecified != null)
            {
                setSpecified(source, true);
            }
            return innerProperty.DeserializeImpl(GetValue(source), context);
        }
    }
}
