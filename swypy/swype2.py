# -*- coding: utf-8 -*-
from levenshtein import levenshtein
from operator import itemgetter
import swypehint as sh

WORDS = open('wordlist.txt').read().split()


def prune_word(query, word):
    return not word[:1] in query[:1]

def prune_swipehint(query, swipehint):
    if len(query) > len(swipehint)+3:
        return True
    return False
    
    

def edit_distance(word1, word2):
    return levenshtein(word1, word2)

def get_suggestions(query, n=5):
    results = []

    for word in WORDS:
        if prune_word(query, word):
            continue
        
        hint = sh.swipehint(word)
        
        if prune_swipehint(query, hint):
            continue
        
        distance = edit_distance(query, hint)
        results.append( (distance, word))
        
    results = sorted(results,key=itemgetter(0))
    
    return results[:n]
    
    
    
if __name__ == '__main__':
    test_cases = ['heqerqllo',               # hello
    'qwertyuihgfcvbnjk',                     # quick
    'wertyuioiuytrtghjklkjhgfd',             # world
    'dfghjioijhgvcftyuioiuytr',              # doctor
    'aserfcvghjiuytedcftyuytre',             # architecture
    'asdfgrtyuijhvcvghuiklkjuytyuytre',      # agriculture
    'mjuytfdsdftyuiuhgvc',                   # music
    'vghjioiuhgvcxsasdvbhuiklkjhgfdsaserty', # vocabulary
    ]
        
    for test in test_cases:
        print test, get_suggestions(test)
    
