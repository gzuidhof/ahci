from __future__ import division
from ast import literal_eval as le
import time
import numpy as np
ngram = {}
bigram = {}

start = time.time()

print "Loading ngram and bigram..."

with open('correctedsingles.txt', 'r') as f: #takes about half a second
	for line in f:
		tup = le(line)
		ngram[tup[0]] = tup[1]

with open('correctedbigrams.txt', 'r') as f:
        for line in f:
            tup = le(line);
            if tup[1] in bigram:
                bigram[tup[1]][tup[2]]  = tup[0]
            else:
                bigram[tup[1]] = {}
                bigram[tup[1]][tup[2]] = tup[0]
            
	
print "Loading done, elpsed time: " + str(time.time()-start)

"""
Assumes only 2 possible words
"""
def uniget(poss):
    
    if(poss[0][1] in ngram and poss[1][1] in ngram and ngram[poss[0][1]] > ngram[poss[1][1]]):
        return poss
    else:
        return poss[::-1] #reversed poss
        
def find(poss,result1):
    for each in result1:
        if each[0] == poss:
            return each

"""
Assumes only 2 possible words
"""
def biget(prev,poss):       
    if prev not in bigram:
        return poss;

    proba = bigram[prev]
    
    if poss[0][1] in proba and poss[1][1] in proba and proba[poss[0][1]] > ngram[poss[1][1]]:
        return poss
    else:
        return poss[::-1]
    

def decide(poss, prev = ""):
    if not prev=="":    
         return biget(prev, poss)
    else:
        return uniget(poss)

#print(getbest('',['aardvark', 'aback', 'abaft']))
#print(getbest('the',['abalone','abbess','abbe','abbot','aardvark','above']))
#'abalone','abbess','abbe','abbot',

# Code below has been omitted but may still be useful later on:
#def getmax(words):
#    best = ('',0)
#    for each in words:
#        if each[1] > best[1]:
#            best = each
#    return best[0]
#
#def getmax2(bigrams):
#    best = ('',0)
#    for each in bigrams:
#        if each[1]>best[1]:
#            best = each
#    return best[0] 