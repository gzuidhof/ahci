# -*- coding: utf-8 -*-
from levenshtein import levenshtein
from operator import itemgetter
import math
import swypehint as sh
import multiprocessing
import pylev

WORDS = open('wordlist.txt').read().split()
SWYPE_HINTS = [sh.swipehint(word) for word in WORDS]

MULTIPROCESS = True
N_PROCESSES = multiprocessing.cpu_count()

def prune_word(query, word):
    return not word[:1] in query[:2] #or not word[-1] == query[-1]

def prune_swipehint(query, swipehint):
    if abs(len(query) - len(swipehint)) > 6:
        return True
    return False

def edit_distance(word1, word2):
    return pylev.levenshtein(word1, word2)

#Score of a word (edit distance, or pruned => None)
def score(query_word_hint_tup):
    query, word, hint = query_word_hint_tup
    if prune_word(query, word):
        return None
    if prune_swipehint(query, hint):
        return None

    return edit_distance(query, hint), word


def get_suggestions(query, n=5):
    results = []
    todo = zip([query]*len(WORDS), WORDS,SWYPE_HINTS)

    if MULTIPROCESS:
        results = pool.map(score, todo)
    else:
        results = map(score, todo)

    results = filter(None, results)
    results = sorted(results,key=itemgetter(0))

    return results[:n]

def init():
    global pool
    pool = multiprocessing.Pool(processes=N_PROCESSES)

def run_test_cases():
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
        print word,query
        print get_suggestions(query, 5), '\n'

#python -m timeit -s "import swype2; swype2.init()" "swype2.run_test_cases()"

if __name__ == '__main__':
    init()
    run_test_cases()
