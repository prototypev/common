namespace Xyun.Xylona.Common.Utilities
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Xyun.Xylona.Common.Comparers;

    /// <summary>
    ///     Fast deep clone utility using expression trees. This is originally from http://blog.nuclex-games.com/mono-dotnet/fast-deep-cloning/ with some changes to support circular references as well as optional shallow copying.
    /// </summary>
    public static class DeepCloner
    {
        #region Static Fields

        /// <summary>
        /// Compiled cloners that perform deep clone operations.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Func<object, Dictionary<object, object>, object>> TypeCloners = new ConcurrentDictionary<Type, Func<object, Dictionary<object, object>, object>>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Creates a deep clone of the specified object, also creating clones of all child objects being referenced.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the object that will be cloned.
        /// </typeparam>
        /// <param name="original">
        /// Object that will be cloned.
        /// </param>
        /// <returns>
        /// A deep clone of the provided object.
        /// </returns>
        public static T DeepClone<T>(T original)
        {
            Func<object, Dictionary<object, object>, object> creator = GetTypeCloner(typeof(T));
            return (T)creator(original, new Dictionary<object, object>());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the existing clone method for the specified type or compiles one if none exists for the type yet.
        /// </summary>
        /// <param name="clonedType">
        /// Type for which a clone method will be retrieved.
        /// </param>
        /// <returns>
        /// The clone method for the specified type.
        /// </returns>
        private static Func<object, Dictionary<object, object>, object> GetTypeCloner(Type clonedType)
        {
            return TypeCloners.GetOrAdd(clonedType, type => new CloneExpressionBuilder(type).CreateTypeCloner());
        }

        #endregion

        /// <summary>
        /// Helper class for generating clone expressions.
        /// </summary>
        private static class CloneExpressionHelper
        {
            #region Static Fields

            /// <summary>
            /// The Dictionary.ContainsKey method info.
            /// </summary>
            private static readonly MethodInfo DictionaryContainsKeyMethodInfo = typeof(Dictionary<object, object>).GetMethod("ContainsKey");

            /// <summary>
            /// The Dictionary.get_Item method info.
            /// </summary>
            private static readonly MethodInfo DictionaryGetItemMethodInfo = typeof(Dictionary<object, object>).GetMethod("get_Item");

            /// <summary>
            /// The FieldInfo.SetValue method info.
            /// </summary>
            private static readonly MethodInfo FieldInfoSetValueMethodInfo = typeof(FieldInfo).GetMethod("SetValue", new[] { typeof(object), typeof(object) });

            /// <summary>
            /// The DeepCloner.GetTypeCloner method info.
            /// </summary>
            private static readonly MethodInfo GetTypeClonerMethodInfo = typeof(DeepCloner).GetMethod("GetTypeCloner", BindingFlags.NonPublic | BindingFlags.Static);

            /// <summary>
            /// The GetType method info.
            /// </summary>
            private static readonly MethodInfo GetTypeMethodInfo = typeof(object).GetMethod("GetType");

            /// <summary>
            /// The Invoke method info.
            /// </summary>
            private static readonly MethodInfo InvokeMethodInfo = typeof(Func<object, Dictionary<object, object>, object>).GetMethod("Invoke");

            #endregion

            #region Methods

            /// <summary>
            /// Creates an expression that copies a complex array value from the source to the target.
            ///     The value will be cloned as well using the dictionary to reuse already cloned objects.
            /// </summary>
            /// <param name="sourceField">
            /// The source field.
            /// </param>
            /// <param name="targetField">
            /// The target field.
            /// </param>
            /// <param name="type">
            /// The type.
            /// </param>
            /// <param name="objectDictionary">
            /// The object dictionary.
            /// </param>
            /// <returns>
            /// The expression.
            /// </returns>
            internal static Expression CreateCopyComplexArrayTypeFieldExpression(Expression sourceField, Expression targetField, Type type, Expression objectDictionary)
            {
                return Expression.IfThenElse(Expression.Call(objectDictionary, DictionaryContainsKeyMethodInfo, sourceField), 
                                             Expression.Assign(targetField, Expression.Convert(Expression.Call(objectDictionary, DictionaryGetItemMethodInfo, sourceField), type)), 
                                             Expression.Assign(targetField, Expression.Convert(Expression.Call(Expression.Call(GetTypeClonerMethodInfo, Expression.Call(sourceField, GetTypeMethodInfo)), InvokeMethodInfo, sourceField, objectDictionary), type)));
            }

            /// <summary>
            /// Creates an expression that copies a complex value from the source to the target.
            ///     The value will be cloned as well using the dictionary to reuse already cloned objects.
            /// </summary>
            /// <param name="original">
            /// The original.
            /// </param>
            /// <param name="clone">
            /// The clone.
            /// </param>
            /// <param name="fieldInfo">
            /// The field information.
            /// </param>
            /// <param name="objectDictionary">
            /// The object dictionary.
            /// </param>
            /// <returns>
            /// The expression.
            /// </returns>
            internal static Expression CreateCopyComplexFieldExpression(Expression original, Expression clone, FieldInfo fieldInfo, ParameterExpression objectDictionary)
            {
                Expression originalField = Expression.Field(original, fieldInfo);

                return Expression.IfThenElse(Expression.Call(objectDictionary, DictionaryContainsKeyMethodInfo, originalField), 
                                             CreateSetFieldExpression(clone, Expression.Convert(Expression.Call(objectDictionary, DictionaryGetItemMethodInfo, originalField), fieldInfo.FieldType), fieldInfo), 
                                             CreateSetFieldExpression(clone, Expression.Convert(Expression.Call(Expression.Call(GetTypeClonerMethodInfo, Expression.Call(originalField, GetTypeMethodInfo)), InvokeMethodInfo, originalField, objectDictionary), fieldInfo.FieldType), fieldInfo));
            }

            /// <summary>
            /// Creates an expression that copies a value from the original to the clone.
            /// </summary>
            /// <param name="original">
            /// The original.
            /// </param>
            /// <param name="clone">
            /// The clone.
            /// </param>
            /// <param name="fieldInfo">
            /// The field info.
            /// </param>
            /// <returns>
            /// The expression that copies a value from the original to the clone.
            /// </returns>
            internal static Expression CreateCopyFieldExpression(Expression original, Expression clone, FieldInfo fieldInfo)
            {
                return CreateSetFieldExpression(clone, Expression.Field(original, fieldInfo), fieldInfo);
            }

            /// <summary>
            /// Creates an expression that sets a value into a field.
            /// </summary>
            /// <param name="clone">
            /// The clone.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <param name="fieldInfo">
            /// The field information.
            /// </param>
            /// <returns>
            /// The expression.
            /// </returns>
            internal static Expression CreateSetFieldExpression(Expression clone, Expression value, FieldInfo fieldInfo)
            {
                // workaround for readonly fields: use reflection, this is a lot slower but the only way except using IL directly
                if (fieldInfo.IsInitOnly)
                {
                    return Expression.Call(Expression.Constant(fieldInfo), FieldInfoSetValueMethodInfo, clone, Expression.Convert(value, typeof(object)));
                }

                return Expression.Assign(Expression.Field(clone, fieldInfo), value);
            }

            #endregion
        }

        /// <summary>
        /// Builder for clone expressions.
        /// </summary>
        private class CloneExpressionBuilder
        {
            #region Static Fields

            /// <summary>
            /// The Array.Clone method info.
            /// </summary>
            private static readonly MethodInfo ArrayCloneMethodInfo = typeof(Array).GetMethod("Clone");

            /// <summary>
            /// The Array.GetLength method info.
            /// </summary>
            private static readonly MethodInfo ArrayGetLengthMethodInfo = typeof(Array).GetMethod("GetLength");

            /// <summary>
            /// The Dictionary.Add method info.
            /// </summary>
            private static readonly MethodInfo DictionaryAddMethodInfo = typeof(Dictionary<object, object>).GetMethod("Add");

            /// <summary>
            /// The FormatterServices.GetUninitializedObject method info.
            /// </summary>
            private static readonly MethodInfo GetUninitializedObjectMethodInfo = typeof(FormatterServices).GetMethod("GetUninitializedObject", BindingFlags.Static | BindingFlags.Public);

            #endregion

            #region Fields

            /// <summary>
            /// The expressions.
            /// </summary>
            private readonly List<Expression> _expressions = new List<Expression>();

            /// <summary>
            /// The object dictionary.
            /// </summary>
            private readonly ParameterExpression _objectDictionary = Expression.Parameter(typeof(Dictionary<object, object>), "objectDictionary");

            /// <summary>
            /// The original expression.
            /// </summary>
            private readonly ParameterExpression _original = Expression.Parameter(typeof(object), "original");

            /// <summary>
            /// The type being cloned.
            /// </summary>
            private readonly Type _type;

            /// <summary>
            /// The variables.
            /// </summary>
            private readonly List<ParameterExpression> _variables = new List<ParameterExpression>();

            /// <summary>
            /// The clone expression.
            /// </summary>
            private ParameterExpression _clone;

            /// <summary>
            /// The typed original expression.
            /// </summary>
            private ParameterExpression _typedOriginal;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="CloneExpressionBuilder"/> class.
            /// </summary>
            /// <param name="type">
            /// The type being cloned.
            /// </param>
            internal CloneExpressionBuilder(Type type)
            {
                this._type = type;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Creates the type cloner.
            /// </summary>
            /// <returns>The clone method for the specified type.</returns>
            internal Func<object, Dictionary<object, object>, object> CreateTypeCloner()
            {
                Expression resultExpression;

                if (IsTypePrimitiveOrString(this._type))
                {
                    this._expressions.Add(this._original);
                    resultExpression = this._expressions[0];
                }
                else
                {
                    this._expressions.Add(this._objectDictionary);

                    // To access the fields of the original type, we need it to be of the actual type instead of an object, so perform a downcast
                    this._typedOriginal = Expression.Variable(this._type);
                    this._variables.Add(this._typedOriginal);
                    this._expressions.Add(Expression.Assign(this._typedOriginal, Expression.Convert(this._original, this._type)));

                    if (this._type.IsArray)
                    {
                        this.CloneArray();
                    }
                    else
                    {
                        this.CloneObject();
                    }

                    resultExpression = Expression.Block(this._variables, this._expressions);
                }

                if (this._type.IsValueType)
                {
                    resultExpression = Expression.Convert(resultExpression, typeof(object));
                }

                return Expression.Lambda<Func<object, Dictionary<object, object>, object>>(resultExpression, this._original, this._objectDictionary)
                                 .Compile();
            }

            /// <summary>
            /// Generates state transfer expressions to copy an array of primitive types.
            /// </summary>
            /// <param name="elementType">
            /// Type of array that will be cloned.
            /// </param>
            /// <param name="source">
            /// Variable expression for the original array.
            /// </param>
            /// <returns>
            /// The variable holding the cloned array.
            /// </returns>
            private static Expression GenerateFieldBasedPrimitiveArrayTransferExpressions(Type elementType, Expression source)
            {
                return Expression.Convert(Expression.Call(Expression.Convert(source, typeof(Array)), ArrayCloneMethodInfo), elementType);
            }

            /// <summary>
            /// Returns all the fields of a type, working around a weird reflection issue
            ///     where explicitly declared fields in base classes are returned, but not
            ///     automatic property backing fields.
            /// </summary>
            /// <param name="type">
            /// Type whose fields will be returned.
            /// </param>
            /// <param name="bindingFlags">
            /// Binding flags to use when querying the fields.
            /// </param>
            /// <param name="skipCloneFields">
            /// The fields to skip deep cloning.
            /// </param>
            /// <returns>
            /// All of the type's fields, including its base types.
            /// </returns>
            private static IEnumerable<FieldInfo> GetFieldInfosIncludingBaseClasses(Type type, BindingFlags bindingFlags, out ISet<FieldInfo> skipCloneFields)
            {
                skipCloneFields = new HashSet<FieldInfo>(new LambdaComparer<FieldInfo>((field1, field2) => field1.DeclaringType == field2.DeclaringType && field1.Name == field2.Name));
                HashSet<FieldInfo> fieldsToClone = new HashSet<FieldInfo>(new LambdaComparer<FieldInfo>((field1, field2) => field1.DeclaringType == field2.DeclaringType && field1.Name == field2.Name));

                while (type != typeof(object) && type != null)
                {
                    FieldInfo[] fields = type.GetFields(bindingFlags);
                    foreach (FieldInfo field in fields)
                    {
                        if (field.IsDefined(typeof(ForceShallowCopyAttribute), true))
                        {
                            skipCloneFields.Add(field);
                            continue;
                        }

                        if (field.Name.StartsWith("<") && field.Name.EndsWith(">k__BackingField"))
                        {
                            PropertyInfo propertyInfo = type.GetProperty(field.Name.Substring(1, field.Name.IndexOf(">k__BackingField", StringComparison.Ordinal) - 1));
                            if (propertyInfo != null)
                            {
                                if (propertyInfo.IsDefined(typeof(ForceShallowCopyAttribute), true))
                                {
                                    skipCloneFields.Add(field);
                                    continue;
                                }
                            }
                        }

                        fieldsToClone.Add(field);
                    }

                    type = type.BaseType;
                }

                return fieldsToClone;
            }

            /// <summary>
            /// Determines whether the specified type is primitive or a string.
            /// </summary>
            /// <param name="type">
            /// The type to check.
            /// </param>
            /// <returns>
            /// <c>true</c> if the type is primitive of a string; otherwise <c>false</c>.
            /// </returns>
            private static bool IsTypePrimitiveOrString(Type type)
            {
                return type.IsPrimitive || type == typeof(string);
            }

            /// <summary>
            ///     Clones the array.
            /// </summary>
            private void CloneArray()
            {
                // Arrays need to be cloned element-by-element
                Type elementType = this._type.GetElementType();

                this._expressions.Add(IsTypePrimitiveOrString(elementType)
                                          ? GenerateFieldBasedPrimitiveArrayTransferExpressions(this._type, this._original)
                                          : this.GenerateFieldBasedComplexArrayTransferExpressions(this._type, elementType, this._typedOriginal, this._variables, this._expressions));
            }

            /// <summary>
            ///     Clones the object.
            /// </summary>
            private void CloneObject()
            {
                // We need a variable to hold the clone because due to the assignments it won't be last in the block when we're finished
                this._clone = Expression.Variable(this._type);
                this._variables.Add(this._clone);

                this._expressions.Add(Expression.Block(Expression.Assign(this._clone, Expression.Convert(Expression.Call(GetUninitializedObjectMethodInfo, Expression.Constant(this._type)), this._type)), // create new instance and add to objectDictionary
                                                       Expression.Call(this._objectDictionary, DictionaryAddMethodInfo, this._original, Expression.Convert(this._clone, typeof(object)))));

                // Generate the expressions required to transfer the type field by field
                this.GenerateFieldBasedComplexTypeTransferExpressions(this._type, this._typedOriginal, this._clone, this._expressions);

                // Make sure the clone is the last thing in the block to set the return value
                this._expressions.Add(this._clone);
            }

            /// <summary>
            /// Generates state transfer expressions to copy an array of complex types.
            /// </summary>
            /// <param name="arrayType">
            /// Type of array that will be cloned.
            /// </param>
            /// <param name="elementType">
            /// Type of the elements of the array.
            /// </param>
            /// <param name="originalArray">
            /// Variable expression for the original array.
            /// </param>
            /// <param name="arrayVariables">
            /// Receives variables used by the transfer expressions.
            /// </param>
            /// <param name="arrayExpressions">
            /// Receives the generated transfer expressions.
            /// </param>
            /// <returns>
            /// The variable holding the cloned array.
            /// </returns>
            private ParameterExpression GenerateFieldBasedComplexArrayTransferExpressions(Type arrayType, Type elementType, Expression originalArray, ICollection<ParameterExpression> arrayVariables, ICollection<Expression> arrayExpressions)
            {
                // We need a temporary variable in order to transfer the elements of the array
                ParameterExpression arrayClone = Expression.Variable(arrayType);
                arrayVariables.Add(arrayClone);

                int dimensionCount = arrayType.GetArrayRank();

                List<ParameterExpression> lengths = new List<ParameterExpression>();
                List<ParameterExpression> indexes = new List<ParameterExpression>();
                List<LabelTarget> labels = new List<LabelTarget>();

                // Retrieve the length of each of the array's dimensions
                for (int index = 0; index < dimensionCount; ++index)
                {
                    // Obtain the length of the array in the current dimension
                    lengths.Add(Expression.Variable(typeof(int)));
                    arrayVariables.Add(lengths[index]);
                    arrayExpressions.Add(Expression.Assign(lengths[index], Expression.Call(originalArray, ArrayGetLengthMethodInfo, Expression.Constant(index))));

                    // Set up a variable to index the array in this dimension
                    indexes.Add(Expression.Variable(typeof(int)));
                    arrayVariables.Add(indexes[index]);

                    // Also set up a label than can be used to break out of the dimension's transfer loop
                    labels.Add(Expression.Label());
                }

                // Create a new (empty) array with the same dimensions and lengths as the original
                arrayExpressions.Add(Expression.Assign(arrayClone, Expression.NewArrayBounds(elementType, lengths)));

                // Initialize the indexer of the outer loop (indexers are initialized one up
                // in the loops (ie. before the loop using it begins), so we have to set this
                // one outside of the loop building code.
                arrayExpressions.Add(Expression.Assign(indexes[0], Expression.Constant(0)));

                // Build the nested loops (one for each dimension) from the inside out
                Expression innerLoop = null;
                for (int index = dimensionCount - 1; index >= 0; --index)
                {
                    List<ParameterExpression> loopVariables = new List<ParameterExpression>();
                    List<Expression> loopExpressions = new List<Expression>
                                                           {
                                                               Expression.IfThen(Expression.GreaterThanOrEqual(indexes[index], lengths[index]), Expression.Break(labels[index]))
                                                           };

                    // If we reached the end of the current array dimension, break the loop
                    if (innerLoop == null)
                    {
                        // The innermost loop clones an actual array element
                        if (IsTypePrimitiveOrString(elementType))
                        {
                            loopExpressions.Add(Expression.Assign(Expression.ArrayAccess(arrayClone, indexes), Expression.ArrayAccess(originalArray, indexes)));
                        }
                        else if (elementType.IsValueType)
                        {
                            this.GenerateFieldBasedComplexTypeTransferExpressions(elementType, Expression.ArrayAccess(originalArray, indexes), Expression.ArrayAccess(arrayClone, indexes), loopExpressions);
                        }
                        else
                        {
                            List<ParameterExpression> nestedVariables = new List<ParameterExpression>();
                            List<Expression> nestedExpressions = new List<Expression>();

                            // A nested array should be cloned by directly creating a new array (not invoking a cloner) since you cannot derive from an array
                            if (elementType.IsArray)
                            {
                                Type nestedElementType = elementType.GetElementType();
                                Expression clonedElement = IsTypePrimitiveOrString(nestedElementType)
                                                               ? GenerateFieldBasedPrimitiveArrayTransferExpressions(elementType, Expression.ArrayAccess(originalArray, indexes))
                                                               : this.GenerateFieldBasedComplexArrayTransferExpressions(elementType, nestedElementType, Expression.ArrayAccess(originalArray, indexes), nestedVariables, nestedExpressions);

                                nestedExpressions.Add(Expression.Assign(Expression.ArrayAccess(arrayClone, indexes), clonedElement));
                            }
                            else
                            {
                                nestedExpressions.Add(CloneExpressionHelper.CreateCopyComplexArrayTypeFieldExpression(Expression.ArrayAccess(originalArray, indexes), Expression.ArrayAccess(arrayClone, indexes), elementType, this._objectDictionary));
                            }

                            // Whether array-in-array of reference-type-in-array, we need a null check before // doing anything to avoid NullReferenceExceptions for unset members
                            loopExpressions.Add(Expression.IfThen(Expression.NotEqual(Expression.ArrayAccess(originalArray, indexes), Expression.Constant(null)), 
                                                                  Expression.Block(nestedVariables, nestedExpressions)));
                        }
                    }
                    else
                    {
                        // Outer loops of any level just reset the inner loop's indexer and execute the inner loop
                        loopExpressions.Add(Expression.Assign(indexes[index + 1], Expression.Constant(0)));
                        loopExpressions.Add(innerLoop);
                    }

                    // Each time we executed the loop instructions, increment the indexer
                    loopExpressions.Add(Expression.PreIncrementAssign(indexes[index]));

                    // Build the loop using the expressions recorded above
                    innerLoop = Expression.Loop(Expression.Block(loopVariables, loopExpressions), labels[index]);
                }

                // After the loop builder has finished, the innerLoop variable contains the entire hierarchy of nested loops, so add this to the clone expressions.
                arrayExpressions.Add(innerLoop);

                return arrayClone;
            }

            /// <summary>
            /// Generates state transfer expressions to copy a complex type.
            /// </summary>
            /// <param name="complexType">
            /// Complex type that will be cloned.
            /// </param>
            /// <param name="source">
            /// Variable expression for the original instance.
            /// </param>
            /// <param name="target">
            /// Variable expression for the cloned instance.
            /// </param>
            /// <param name="expression">
            /// Receives the generated transfer expressions.
            /// </param>
            private void GenerateFieldBasedComplexTypeTransferExpressions(Type complexType, Expression source, Expression target, ICollection<Expression> expression)
            {
                // Enumerate all of the type's fields and generate transfer expressions for each
                ISet<FieldInfo> skipCloneFieldInfos;
                IEnumerable<FieldInfo> fieldInfos = GetFieldInfosIncludingBaseClasses(complexType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, out skipCloneFieldInfos);

                // For those field which skip deep copying, do shallow copying by assigning the field value from the source to the target
                foreach (FieldInfo fieldInfo in skipCloneFieldInfos)
                {
                    expression.Add(CloneExpressionHelper.CreateCopyFieldExpression(source, target, fieldInfo));
                }

                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    Type fieldType = fieldInfo.FieldType;

                    if (IsTypePrimitiveOrString(fieldType))
                    {
                        expression.Add(CloneExpressionHelper.CreateCopyFieldExpression(source, target, fieldInfo));
                    }
                    else if (fieldType.IsValueType)
                    {
                        // A nested value type is part of the parent and will have its fields directly assigned without boxing, new instance creation or anything like that.
                        this.GenerateFieldBasedComplexTypeTransferExpressions(fieldType, Expression.Field(source, fieldInfo), Expression.Field(target, fieldInfo), expression);
                    }
                    else
                    {
                        this.GenerateFieldBasedReferenceTypeTransferExpressions(source, target, expression, fieldInfo);
                    }
                }
            }

            /// <summary>
            /// Generates the expressions to transfer a reference type (array or class).
            /// </summary>
            /// <param name="original">
            /// Original value that will be cloned.
            /// </param>
            /// <param name="clone">
            /// Variable that will receive the cloned value.
            /// </param>
            /// <param name="expressions">
            /// Receives the expression generated to transfer the values.
            /// </param>
            /// <param name="fieldInfo">
            /// Reflection information about the field being cloned.
            /// </param>
            private void GenerateFieldBasedReferenceTypeTransferExpressions(Expression original, Expression clone, ICollection<Expression> expressions, FieldInfo fieldInfo)
            {
                // Reference types and arrays require special care because they can be null, so gather the transfer expressions in a separate block for the null check
                List<Expression> fieldExpressions = new List<Expression>();
                List<ParameterExpression> fieldVariables = new List<ParameterExpression>();

                Type fieldType = fieldInfo.FieldType;

                if (fieldType.IsArray)
                {
                    Expression fieldClone = this.GenerateFieldBasedComplexArrayTransferExpressions(fieldType, fieldType.GetElementType(), Expression.Field(original, fieldInfo), fieldVariables, fieldExpressions);
                    fieldExpressions.Add(CloneExpressionHelper.CreateSetFieldExpression(clone, fieldClone, fieldInfo));
                }
                else
                {
                    fieldExpressions.Add(CloneExpressionHelper.CreateCopyComplexFieldExpression(original, clone, fieldInfo, this._objectDictionary));
                }

                expressions.Add(Expression.IfThen(Expression.NotEqual(Expression.Field(original, fieldInfo), Expression.Constant(null)), 
                                                  Expression.Block(fieldVariables, fieldExpressions)));
            }

            #endregion
        }
    }
}