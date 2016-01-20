from operator import itemgetter

WORDS = open('wordlist.txt').read().split()
OTHERWORDS = open('en.txt', encoding='utf8').read().splitlines()
BIGRAMS = open('w2_.txt', encoding='latin-1').read().splitlines()
print(BIGRAMS[0])

file = open("testbigrams","w")

##########################
# intersect two lists for unigram probabilities
def intersect(a, b):
    return list(set([t[0] for t in a]) & set(b))

# intersect two lists for bigram probabilities
def intersect2(a, b):
    return list(set([t[2] for t in a]) & set(b))

# add probabilities to selected word and
# prune all unused words from the longest list.
def pruneprob(each):
    for other in OTHERWORDS:
        if each == other[0]:
            j=0
            while j<OTHERWORDS.index(other):
                OTHERWORDS.remove(OTHERWORDS[j])
            return((each,other[1]))

# add probabilities to selected word and
# prune all unused words from the longest list.
def pruneprob2(each):
    for other in BIGRAMS:
        if each == other[2]:
            #BIGRAMS[0:BIGRAMS.index(other)-1] = []
            return((other[0],other[1],each))

# write the unigram probabilities for all known words to a .txt file.
def unigram():
    i=0
    for each in OTHERWORDS:
        (word,freq) = OTHERWORDS[i].split(' ')
        OTHERWORDS[i] = (word,float(freq))
        i=i+1
    
    wordfreqs = intersect(OTHERWORDS,WORDS)
    wordfreqs.sort()
    OTHERWORDS.sort()
    
    for each in wordfreqs:
        each1=pruneprob(each)
        wordfreqs[wordfreqs.index(each)]=each1
        file.write(str(each1) + "\n")
    file.close
    
    print("Done!")

# write the bigram probabilities for all known words to a .txt file.
def bigram():
    i=0
    for each in BIGRAMS:
        (freq,word1,word2) = BIGRAMS[i].split('\t')
        BIGRAMS[i] = (float(freq),word1,word2)
        i=i+1
    bifreqs = intersect2(BIGRAMS,WORDS)
    bifreqs.sort()
    sorted(BIGRAMS, key=itemgetter(2))
    
    for each in bifreqs:
        each1=pruneprob2(each)
        bifreqs[bifreqs.index(each)]=each1
    sorted(bifreqs, key=itemgetter(1))
    for each in bifreqs:        
        file.write(str(each) + "\n")
    file.close
    sorted(bifreqs, key=itemgetter(1))    
    print(bifreqs[0:10])
    
    print("Done!")# + str(len(bifreqs))+ " from " + str(len(BIGRAMS)))

# write the trigram probabilities for all known words to a .txt file.
#def trigram():
    # do above but for three words

#############################

#unigram()
bigram()
#trigram()
