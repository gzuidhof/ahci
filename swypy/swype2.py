from __future__ import division
from operator import itemgetter
import math
import swypehint as sh
import Levenshtein as lv #python-Levenshtein package
import numpy as np
import time
import cPickle as pickle
import os.path
from itertools import izip
from tqdm import tqdm

WORDS = open('wordlist.txt').read().split()
WORDS = sorted(WORDS)
N_HINTS = [sh.n_swipehints_opts(word)[0] for word in WORDS]
Emptylist=True
SWYPE_HINTS = []
SWYPE_HINTS2=[]
N_FRACTIONS = 50
FRACTIONS = np.linspace(0,1,N_FRACTIONS)

swype_hints_filepath="word_paths_{0}.p".format(N_FRACTIONS)

if os.path.isfile(swype_hints_filepath):
    with open(swype_hints_filepath, 'rb') as f:
        print "Initializing, loading swype paths from file"
        SWYPE_HINTS = pickle.load(f)
        print "Done"
else:
    print "Initializing, generating many swype paths (will be persisted to disk)"
    for word, n_hints in tqdm(zip(WORDS, N_HINTS)):
        hints = []
        for fraction in FRACTIONS:
            index = int(fraction*n_hints)
            hints.append(sh.nth_swipehint((word, index)))
        SWYPE_HINTS.append(hints)
    with open(swype_hints_filepath, 'wb') as f:
        print "Writing to file", swype_hints_filepath
        pickle.dump(SWYPE_HINTS, f)
        print "Done"

def prune_word(query, word):
    return not word[:1] in query[:2] #or not word[-1] == query[-1]

def prune_swipehint(query, swipehint):
    if abs(len(query) - len(swipehint)) > 4:
        return True
    return False

def edit_distance(word1, word2):
    return lv.distance(word1, word2)

#Score of a word (edit distance)
def score(query, word, hints,i):
    if prune_word(query, word):
        return 20000, word, hints #Arbitrary very high number.

    if prune_swipehint(query, hints[i]):
        return 10000, word, hints #Arbitrary very high number.

    return edit_distance(query, hints[i]), word, hints

def get_suggestions(query, durations, text, n=5):

    #word_todo = WORDS
    score_avg = np.zeros(10)
    score_min = np.ones(10) * 99999
    get_fitting_words(query,durations)

    word_todo = sorted(open('wordlist2.txt').read().split())
    hints=SWYPE_HINTS2
    print(len(SWYPE_HINTS2), len(word_todo))
    if len(word_todo)<1:
        word_todo=WORDS
        hints = SWYPE_HINTS
        print "Word not found using timings, whole word list is used!"

    for i in xrange(len(FRACTIONS)):
        results = [score(query, word, hint, i) for word, hint in izip(word_todo, hints)]

        if len(results) != 10:
            results = sorted(results,key=itemgetter(0))
            results = results[:10]

            results = sorted(results, key=itemgetter(1))

        scores, word_todo, hints = izip(*results)
        score_avg += scores
        score_min = np.minimum(score_min,scores)

    score_avg = score_avg / len(FRACTIONS) #Actual average now

    scores = (0.8*score_min)+(0.2*score_avg)
    results = zip(scores, word_todo)
    results = sorted(results, key=itemgetter(0))

    results = [(int(x), y) for (x, y) in results]
    return results[:n]

def get_fitting_words(charlist,timelist): #(wordlist, list of characters from swipe, list of timings )
    assert (len(charlist) == len(timelist)),"lists are not the same length!"
    WORDS2 = open('wordlist2.txt','w')
    durations=[float(x) for x in timelist if not x=='']
    mean=np.mean(durations) #possibly not actual avarage/std but works ;).
    std=np.std(durations)
    char_in_word=[]
    inword=False
    for i,f in enumerate(durations):
        if f >= mean + 0.5* std:
            char_in_word.append(charlist[i])
    for k, word in enumerate(WORDS):
        inword=True
        w=word
        for x in char_in_word:
            if x not in w:
                inword=False
            else:
                for i,j in enumerate(w):
                    if j == x:
                        w=w[i:]
                        break



        if inword:
            SWYPE_HINTS2.append(SWYPE_HINTS[k])
            WORDS2.write(word+"\n")


    print "chars in word:", char_in_word
    WORDS2.close

def run_test_cases():

    test_cases = ['hytrerfghjkllo',          # hello
    'qwertyuihgfcvbnjk',                     # quick
    'wertyuioiuytrtghjklkjhgfd',             # world
    'dfghjioijhgvcftyuioiuytr',              # doctor
    'aserfcvghjiuytedcftyuytre',             # architecture
    'asdfgrtyuijhvcvghuiklkjuytyuytre',      # agriculture
    'mjuytfdsdfghuijnbvc',                   # music
    'vghjioiuhgvcxsasdvbhuiklkjhgfdsaserty', # vocabulary
    'tredfgbnbgfds'
    ]

    actual = ['hello','quick','world','doctor','architecture',
        'agriculture','music','vocabulary', 'trends']

    for query, word in zip(test_cases,actual):
        t = time.time()
        print word,query
        print get_suggestions(query, 0, 0, 5), "{:5.1f}ms".format((time.time()-t)*1000), '\n'

#Use this for benchmarking
#python -m timeit -s "import swype2; swype2.init()" "swype2.run_test_cases()"

if __name__ == '__main__':
    run_test_cases()