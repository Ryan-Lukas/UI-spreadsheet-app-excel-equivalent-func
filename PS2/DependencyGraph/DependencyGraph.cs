// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private Dictionary<string, HashSet<string>> dependees;
        private Dictionary<string, HashSet<string>> dependents;
        private int sizeOfGraph;
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependees = new Dictionary<string, HashSet<string>>();
            dependents = new Dictionary<string, HashSet<string>>();
            sizeOfGraph = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return sizeOfGraph; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                if (dependents.Count == 0)
                {
                    return 0;
                }


                return dependents[s].Count;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (!dependees.ContainsKey(s)) { return false; }
            return dependees[s].Count == 0 ? false : true;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty. 
        /// </summary> 
        public bool HasDependees(string s)
        {
            if (!dependents.ContainsKey(s)) { return false; }
            return dependents[s].Count == 0 ? false : true;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {


            return dependees.TryGetValue(s, out HashSet<string> dependeesOfS) ? dependeesOfS : (dependeesOfS = new HashSet<string>()); //if links are there, return links, otherwise return new hashset of 0
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            return dependents.TryGetValue(s, out HashSet<string> dependentsOfS) ? dependentsOfS : (dependentsOfS = new HashSet<string>()); //if links are there, return links, otherwise return new hashset of 0
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            if (s == null || s == "") { return; } //won't create pair if there is no t value
            if (t == null || t == "") { return; } //won't create pair if there is no t value

            //if there is s in dependees otherwise go to else
            if (dependees.ContainsKey(s))
            {

                //if the s value already has the t value, don't add it, otherwise add it
                if (!dependees[s].Contains(t))
                {
                    dependees[s].Add(t);

                    //see if t is already in dependents list
                    if (dependents.ContainsKey(t))
                    {
                        dependents[t].Add(s);
                        sizeOfGraph++;
                    }
                    else
                    {
                        //create new t in dependents
                        HashSet<string> hash = new HashSet<string>();
                        hash = new HashSet<string>();
                        hash.Add(s);
                        dependents.Add(t, hash);
                        sizeOfGraph++;
                    }

                }

            }
            else
            {
                //create new s in dependees
                HashSet<string> hash = new HashSet<string>();
                hash.Add(t);
                dependees.Add(s, hash);
                //if t is in dependets, add link otherwise create new one
                if (dependents.ContainsKey(t))
                {
                    dependents[t].Add(s);
                    sizeOfGraph++;
                }
                else
                {
                    hash = new HashSet<string>();
                    hash.Add(s);
                    dependents.Add(t, hash);
                    sizeOfGraph++;
                }


            }

        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (s == null || s == "") { return; } //won't create pair if there is no t value
            if (t == null || t == "") { return; } //won't create pair if there is no t value

            //if removing in dependees
            if (dependees.ContainsKey(s))
            {
                if (dependees[s].Contains(t))
                {
                    //removing link
                    dependees[s].Remove(t);
                    dependents[t].Remove(s);
                    sizeOfGraph--;

                    //if no more links, remove from list
                    if (!HasDependents(s)) { dependees.Remove(s); }
                    if (!HasDependees(t)) { dependents.Remove(t); }
                }
            }

            //if removing in dependents
            if (dependents.ContainsKey(s))
            {
                if (dependents[s].Contains(t))
                {
                    //removing link
                    dependents[s].Remove(t);
                    dependees[t].Remove(s);
                    sizeOfGraph--;

                    //if no more links, remove from list
                    if (!HasDependents(s)) { dependents.Remove(s); }
                    if (!HasDependees(t)) { dependees.Remove(t); }
                }
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            IEnumerable<string> prevDependents = this.GetDependents(s);
            HashSet<string> takeOutOldValues = new HashSet<string>();
            foreach (string r in prevDependents) { takeOutOldValues.Add(r); }//making a list of all values to take out
            foreach (string r in takeOutOldValues) { RemoveDependency(s, r); }//removing link
            foreach (string t in newDependents) { AddDependency(s, t); }//adding link
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            IEnumerable<string> prevDependees = this.GetDependees(s);
            HashSet<string> takeOutOldValues = new HashSet<string>();
            foreach (string r in prevDependees) { takeOutOldValues.Add(r); }//making a list of all values to take out
            foreach (string r in takeOutOldValues) { RemoveDependency(r, s); }//removing link
            foreach (string t in newDependees) { AddDependency(t, s); }//adding link
        }



    }

}