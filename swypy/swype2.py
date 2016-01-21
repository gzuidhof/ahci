# -*- coding: utf-8 -*-
from operator import itemgetter
import math
import swypehint as sh
import numpy as np
import lookup as lu
from multiprocessing import Pool, cpu_count
from multiprocessing.pool import ThreadPool
import Levenshtein as lv #python-Levenshtein package

WORDS = open('wordlist.txt').read().split()
SWYPE_HINTS = [sh.swipehint(word) for word in WORDS]

MULTIPROCESS = False
MULTITHREAD = False
N_PROCESSES = cpu_count()

def prune_word(query, word):
    return not word[:1] in query[:2] #or not word[-1] == query[-1]

def prune_swipehint(query, swipehint):
    if abs(len(query) - len(swipehint)) > 5:
        return True
    return False

def edit_distance(word1, word2):
    #return pylev.levenshtein(word1, word2)
    return lv.distance(word1, word2)

#Score of a word (edit distance, or pruned => None)
def score(query_word_hint_tup):
    query, word, hint = query_word_hint_tup
    if prune_word(query, word):
        return None
    if prune_swipehint(query, hint):
        return None

    return edit_distance(query, hint), word


def get_suggestions(charlist,durations,text, n=5): #charlist,timelist, query, n=5):
    results = []
    get_fitting_words(charlist,durations)
    WORDS=open('wordlist2.txt').read().split()
    
    query="".join(charlist)
    #
    todo = zip([query]*len(WORDS), WORDS,SWYPE_HINTS)

    if MULTIPROCESS:
        results = pool.map(score, todo)
    else:
        results = map(score, todo)

    results = filter(None, results)
    results = sorted(results,key=itemgetter(0))
    return results[:n]

def get_fitting_words(charlist,timelist): #(wordlist, list of characters from swipe, list of timings )
    assert (len(charlist) == len(timelist)),"lists are not the same length!"
    WORDS2 = open('wordlist2.txt','w')
    charlist=charlist[1:]
    timelist=timelist[1:]
    durations=[float(x) for x in timelist if not x=='']
    mean=np.mean(durations)
    std=np.std(durations)
    inword=False
    char_in_word=[]
    print "durations",durations
    print "mean", mean
    print "std", std
    print ""
    for i,f in enumerate(durations):
        if f >= mean + 0.5* std:
            char_in_word.append(charlist[i])
    for word in WORDS:
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
         WORDS2.write(word+"\n")
    WORDS2.close
   






def init():
    global pool
    if MULTITHREAD:
        pool = ThreadPool(processes=N_PROCESSES)
    else:
        pool = Pool(processes=N_PROCESSES)

def run_test_cases():
    import time

    test_cases = ['hytrerfghjkllo',          # hello
    'qwertyuihgfcvbnjk',                     # quick
    'wertyuioiuytrtghjklkjhgfd',             # world
    'dfghjioijhgvcftyuioiuytr',              # doctor
    'aserfcvghjiuytedcftyuytre',             # architecture
    'asdfgrtyuijhvcvghuiklkjuytyuytre',      # agriculture
    'mjuytfdsdftyuiuhgvc',                   # music
    'vghjioiuhgvcxsasdvbhuiklkjhgfdsaserty', # vocabulary
    ]

    actual = ['hello','quick','world','doctor','architecture',
        'agriculture','music','vocabulary']

    for query, word in zip(test_cases,actual):
        t = time.time()
        print word,query
       # print get_suggestions(query, 5), "{:5.1f}ms".format((time.time()-t)*1000), '\n'

#Use this for benchmarking
#python -m timeit -s "import swype2; swype2.init()" "swype2.run_test_cases()"

if __name__ == '__main__':
    init()
    run_test_cases()
