from flask import Flask, jsonify, request
import swype2 as swype
import time


app = Flask(__name__)

@app.route('/suggest', methods=['GET'])
def get_tasks():

    query = request.args.get('query', None)
    print "QUERY:", query
    
    if query is not None and len(query) > 0:
        query = str(query)
        #(charlist,durations,text)=query.split("*")
        #durations=durations.split(";")
        #charlist=charlist.split(";")
        #text=text.split(";")
        print "QUERY2!", query
        t = time.time()
        suggestions = swype.get_suggestions(query, 0, 0)
        #suggestions = swype.get_suggestions("".join(charlist),durations,text) 
        suggestions = zip(*suggestions)[1] if len(suggestions) > 0 else []
        print "Elapsed time getting suggestions {:5.1f}ms".format((time.time()-t)*1000)
        return jsonify({'query':query, 'suggestions':suggestions, 'error':'None'})
    else:
        return jsonify({'error':'No query argument given!', 'suggestions':[],query:None})

if __name__ == '__main__':
    app.run(debug=True)
