﻿using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf.Property;
using System.Diagnostics;

namespace ProtoBuf
{
    internal abstract class SerializerProxy<T>
    {
        public static readonly SerializerProxy<T> Default;

        static SerializerProxy()
        {
            if (Serializer.IsEntityType(typeof(T)))
            {
                Default = (SerializerProxy<T>)typeof(SerializerProxy<T>).GetMethod("MakeItem")
                    .MakeGenericMethod(typeof(T)).Invoke(null, null);
            }
            else // see if our property-factory can handle this type; if it can, use a wrapper object
            {
                try
                {
                    DataFormat fmt = DataFormat.Default;
                    if (PropertyFactory.CreatePassThru<T>(1, ref fmt) != null)
                    {
                        Default = (SerializerProxy<T>)typeof(SerializerProxy<T>).GetMethod("MakeValue")
                            .MakeGenericMethod(typeof(T)).Invoke(null, null);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString(), "SerializerProxy.cctor");
                    // nope, can't do it...
                }
            }
            if(Default == null)
            {
                throw new InvalidOperationException("Only data-contract classes (and lists/arrays of such) can be processed (error processing " + typeof(T).Name + ")");
            }
        }

        public int Serialize(T instance, Stream destination)
        {
            return Serialize(instance, new SerializationContext(destination, null));
        }
        public void Deserialize(ref T instance, Stream source)
        {
            Deserialize(ref instance, new SerializationContext(source, null));
        }
        public abstract int Serialize(T instance, SerializationContext destination);
        public abstract void Deserialize(ref T instance, SerializationContext source);

        public static SerializerProxy<TItem> MakeItem<TItem>()
            where TItem : class, T
        {
            Type actualType = typeof(TItem), baseType = actualType;
            while (Serializer.IsEntityType(baseType.BaseType))
            {
                baseType = baseType.BaseType;
            }
            if(baseType == actualType)
            {
                return new SerializerItemProxy<TItem, TItem>();
            }
            return (SerializerProxy<TItem>) Activator.CreateInstance(
                typeof(SerializerItemProxy<,>).MakeGenericType(baseType, actualType));
        }
        public static SerializerProxy<TValue> MakeValue<TValue>()
        {
            return new SerializerSimpleProxy<TValue>();
        }
    }

    sealed class SerializerItemProxy<TBaseClass, TActualClass> : SerializerProxy<TActualClass>
        where TActualClass : class, TBaseClass
        where TBaseClass : class
    {
        public override int Serialize(TActualClass instance, SerializationContext destination)
        {
            return Serializer<TBaseClass>.SerializeChecked(instance, destination);
        }
        public override void Deserialize(ref TActualClass instance, SerializationContext source)
        {
            TBaseClass tmp = instance;
            Serializer<TBaseClass>.DeserializeChecked<TActualClass>(ref tmp, source);
            instance = (TActualClass) tmp;
        }
    }
    
    sealed class SerializerSimpleProxy<TValue> : SerializerProxy<TValue>
    {
        public override int Serialize(TValue value, SerializationContext destination)
        {
            return Serializer<SimpleWrapper>.SerializeChecked(new SimpleWrapper(value), destination);
        }
        public override void Deserialize(ref TValue value, SerializationContext source)
        {
            SimpleWrapper wrapper = null;
            Serializer<SimpleWrapper>.DeserializeChecked<SimpleWrapper>(ref wrapper, source);
            if (wrapper != null) value = wrapper.Value;
        }

        [ProtoContract]
        public sealed class SimpleWrapper
        {
            public SimpleWrapper() { }
            public SimpleWrapper(TValue value) { this.value = value; }
            private TValue value;
            [ProtoMember(Serializer.ListItemTag)]
            public TValue Value
            {
                get { return this.value; }
                set { this.value = value; }
            }
        }
    }
}
