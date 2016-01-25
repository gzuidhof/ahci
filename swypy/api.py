from flask import Flask, jsonify, request
import swype2 as swype
import time
from ast import literal_eval 


app = Flask(__name__)

@app.route('/suggest', methods=['GET'])
def get_tasks():

    args = request.args.get('query', None)
    print "arguments:", args
    
    result1=[]
    result2=[]
    with open('correctedsingles.txt', 'r') as f:
        for line in f:
            result1.append(literal_eval(line.strip()))  
    with open('correctedbigrams.txt', 'r') as f:
        for line in f:
            result2.append(literal_eval(line.strip()))    
    
    if args is not None and len(args) > 0:
        args = str(args)
        (query,durations,lastWord)=args.split("*")
        durations=durations.split(";")
        print "QUERY2!", query
        t = time.time()
        suggestions = swype.get_suggestions(query, 0, lastWord,result1,result2)
        #suggestions = swype.get_suggestions("".join(charlist),durations,text,result1,result2) 
        suggestions = zip(*suggestions) if len(suggestions) > 0 else []
        print "Elapsed time getting suggestions {:5.1f}ms".format((time.time()-t)*1000)
        return jsonify({'query':query, 'suggestions':suggestions, 'error':'None'})
    else:
        return jsonify({'error':'No query argument given!', 'suggestions':[],query:None})

if __name__ == '__main__':
    app.run(debug=True)
