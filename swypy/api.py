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
        t = time.time()
        suggestions = swype.get_suggestions(query)
        print "Elapsed time getting suggestions {:5.1f}ms".format((time.time()-t)*1000)

        suggestions = zip(*suggestions)[1] if len(suggestions) > 0 else []

        return jsonify({'query':query, 'suggestions':suggestions, 'error':'None'})
    else:
         return jsonify({'error':'No query argument given!', 'suggestions':[],query:None})

if __name__ == '__main__':
    swype.init()
    app.run(debug=True)
