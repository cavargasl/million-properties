/**
 * Utility function to transform DTO objects to domain objects
 */
export function transformDtoObject<TSource, TTarget>(
  source: TSource,
  mapping: Record<keyof TSource, keyof TTarget>
) {
  let result = {} as TTarget;

  for (const sourceKey in mapping) {
    const targetKey = mapping[sourceKey];
    const value = source[sourceKey];
    result[targetKey] = value as unknown as TTarget[keyof TTarget];
  }

  return {
    result: () => result,
    
    nullOrEmptyToUndefined: function() {
      for (const key in result) {
        const value = result[key];
        if (value === null || value === '') {
          result[key] = undefined as TTarget[typeof key];
        }
      }
      return this;
    },
    
    removeUndefined: function() {
      const filtered = {} as TTarget;
      for (const key in result) {
        if (result[key] !== undefined) {
          filtered[key] = result[key];
        }
      }
      result = filtered;
      return this;
    },
  };
}
