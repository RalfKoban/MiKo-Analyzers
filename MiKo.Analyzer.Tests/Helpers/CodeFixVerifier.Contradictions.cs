using System.Collections.Generic;
using System.Linq;

namespace TestHelper
{
    public partial class CodeFixVerifier
    {
        protected static readonly Dictionary<string, string> ContradictionMap = new Dictionary<string, string>
                                                                                    {
                                                                                        { "aren't", "are not" },
                                                                                        { "can't", "cannot" },
                                                                                        { "couldn't", "could not" },
                                                                                        { "daren't", "dare not" },
                                                                                        { "didn't", "did not" },
                                                                                        { "doesn't", "does not" },
                                                                                        { "don't", "do not" },
                                                                                        { "hadn't", "had not" },
                                                                                        { "hasn't", "has not" },
                                                                                        { "haven't", "have not" },
                                                                                        { "isn't", "is not" },
                                                                                        { "needn't", "need not" },
                                                                                        { "shouldn't", "should not" },
                                                                                        { "wasn't", "was not" },
                                                                                        { "weren't", "were not" },
                                                                                        { "won't", "will not" },
                                                                                        { "wouldn't", "would not" },

                                                                                        // capitalized
                                                                                        { "Aren't", "Are not" },
                                                                                        { "Can't", "Cannot" },
                                                                                        { "Couldn't", "Could not" },
                                                                                        { "Daren't", "Dare not" },
                                                                                        { "Didn't", "Did not" },
                                                                                        { "Doesn't", "Does not" },
                                                                                        { "Don't", "Do not" },
                                                                                        { "Hadn't", "Had not" },
                                                                                        { "Hasn't", "Has not" },
                                                                                        { "Haven't", "Have not" },
                                                                                        { "Isn't", "Is not" },
                                                                                        { "Needn't", "Need not" },
                                                                                        { "Shouldn't", "Should not" },
                                                                                        { "Wasn't", "Was not" },
                                                                                        { "Weren't", "Were not" },
                                                                                        { "Won't", "Will not" },
                                                                                        { "Wouldn't", "Would not" },

                                                                                        // without apostrophes
                                                                                        { "cant", "cannot" },
                                                                                        { "couldnt", "could not" },
                                                                                        { "darent", "dare not" },
                                                                                        { "didnt", "did not" },
                                                                                        { "doesnt", "does not" },
                                                                                        { "dont", "do not" },
                                                                                        { "hadnt", "had not" },
                                                                                        { "hasnt", "has not" },
                                                                                        { "havent", "have not" },
                                                                                        { "isnt", "is not" },
                                                                                        { "neednt", "need not" },
                                                                                        { "shouldnt", "should not" },
                                                                                        { "wasnt", "was not" },
                                                                                        { "werent", "were not" },
                                                                                        { "wont", "will not" },
                                                                                        { "wouldnt", "would not" },

                                                                                        // capitalized without apostrophes
                                                                                        { "Cant", "Cannot" },
                                                                                        { "Couldnt", "Could not" },
                                                                                        { "Darent", "Dare not" },
                                                                                        { "Didnt", "Did not" },
                                                                                        { "Doesnt", "Does not" },
                                                                                        { "Dont", "Do not" },
                                                                                        { "Hadnt", "Had not" },
                                                                                        { "Hasnt", "Has not" },
                                                                                        { "Havent", "Have not" },
                                                                                        { "Isnt", "Is not" },
                                                                                        { "Neednt", "Need not" },
                                                                                        { "Shouldnt", "Should not" },
                                                                                        { "Wasnt", "Was not" },
                                                                                        { "Werent", "Were not" },
                                                                                        { "Wont", "Will not" },
                                                                                        { "Wouldnt", "Would not" },
                                                                                    };

        protected static readonly string[] WrongContradictionPhrases = ContradictionMap.Keys.ToArray();
    }
}