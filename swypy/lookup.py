from __future__ import division
from ast import literal_eval as le
import time
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
           # line.append(le(line.strip()))    #working on this
	
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

def biget(prev,poss,result1,result2):       
    indices = [(x[2],x[0]) for x in result2 if x[1] == prev and x[2] in poss]
    theseguys = [each[0] for each in indices]
    for x in poss:
        if not x in theseguys:
            indices.append(find(x,result1))
    indices = sorted(indices,key=lambda x:(-x[1],x[0]))
    result = [x[0] for x in indices]
    return result

def decide(poss, prev = ""):
#    if not prev=="":    
#        if prev in [thisone[1] for thisone in result2]:
#            return biget(prev,poss,result1,result2)
#    else:
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