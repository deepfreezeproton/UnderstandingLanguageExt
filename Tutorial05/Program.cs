﻿using System;
using System.Linq;
using Tutorial05;

namespace Tutorial05
{
    class Program
    {
        static void Main(string[] args)
        {
            // A Box
            Box<int[]> boxOfIntegers = new Box<int[]>(new[] { 3, 5, 7, 9, 11, 13, 15 });
            Box<int[]> boxOfNewIntegers = new Box<int[]>(new[] { 3, 5, 88, 29, 155, 123, 1 });

            // Do something with or to the Box

            var doubled1 = boxOfIntegers
                            .Bind(extract => new Box<int[]>(extract.Select(x => x * 2).ToArray())); // Extract, Validate and transform using Bind()

            var doubled2 = boxOfIntegers
                            .Map(numbers => numbers.Select(x => x * 2).ToArray()); // Extract, Validate and transform using Bind()

            // Extract, Validate and transform using SelectMany()
            var doubled3 = from extract in boxOfIntegers 
                           from transformed in DoubleNumbers(extract) // bind() part of SelectMany() ie transform extracted value 
                select transformed; // project(extract, transformedAndLiftedResult) part of SelectMany
            
            var doubled4 = from extract in boxOfIntegers
                select DoubleNumbers(extract).Extract; // Use Select via linq expression syntax
            
            // Note we can use Map or Bind, but it becomes nessesary to choose/use a specific one depending
            // on if or not the provided transformation function returns a box or not (lifts or doesn't), ie is transformed in a call to Bind() or Map()
            Box<int[]> doubleDouble1 = boxOfIntegers
                .Bind(numbers => DoubleNumbers(numbers))
                .Map(DoubleNumbers)
                .Bind(box => box.Bind( numbers => DoubleNumbers(numbers) ));

            var doubleDouble2 = from numbers in boxOfIntegers
                from redoubled in DoubleNumbers(numbers)
                select redoubled;
                
            
            // Give me a box of Double Double of my Box
            var doubleDouble3 = from firstDoubleTransformation in DoubleMyBox(boxOfIntegers)
                                from secondDoubleTransformation in DoubleNumbers(firstDoubleTransformation) //VET: bind part of SelectMany()
                                    select secondDoubleTransformation; // project(reDouble, firstDouble)
        }

        /// <summary>
        /// Takes a Box of numbers and produces a box of doubled numbers
        /// </summary>
        private static Box<int[]> DoubleMyBox(Box<int[]> boxOfIntegers)
        {
            return from extract in boxOfIntegers
                from doubledNumber in DoubleNumbers(extract)
                select doubledNumber;
        }

        // transform Extracted, and Lift it
        static Box<int[]> DoubleNumbers(int[] extract)
        {
            return new Box<int[]>(extract.Select(x => x * 2).ToArray());
        }
    }
}
