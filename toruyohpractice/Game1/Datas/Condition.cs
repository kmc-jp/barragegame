using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    /// <summary>
    /// a class with various static methods that interprets the string as condition . Comparison inside Number
    /// </summary>
    class Condition: IDisposable
    {
        // ----    Be Careful!!    ----
        // following words are already used:
        /*  &: and;  |: or;  <,>,=: left bigger,smaller,equal to right;  (,):right,left parenthesis;
         *  ,: or other number
        */
        public const string route_set = "-rs";
        public const string duration = "-dr";
        public const string hP = "-hp";
        public const string hPp = "-hPp";
        #region static
        /// <summary>
        /// 構文解析時にこれらの文字列は対応した番号の引数を意味する,今は"-rs"は0番目の引数の値を意味する
        /// </summary>
        static readonly string[] label = new string[]
        {
           route_set,duration, hP,hPp,
        };
        
        #endregion


        string conditions;
        int pos;

        /// <summary>
        /// skillが使用できるかの条件が書かれたstringをもらい,解読して今使えるかを判定する
        /// </summary>
        /// <param name="_routeSet">今の動きのループセットの番号</param>
        /// <param name="_duration">持続時間,周期など、</param>
        /// <param name="_hp">今のlife,負の時は死んでいることを意味する.0の時は死んだを意味する</param>
        /// <param name="_hpP">今のlifeのパーセント、整数100が100%;　0,負はhpと同じ扱い</param>
        /// <param name="indexShift">初期値0、判定に使われる引数を制限したい時に使う,2にすると前の2つは無視</param>
        /// <returns></returns>
        public static bool skillCondition( string c,int _routeSet,int _duration,int _hp,int _hpP, int indexShift=0)
        {
            if (c == null) return true;
            Condition con = new Condition(c);
            bool ans=con.analyze(new int[] { _routeSet, _duration, _hp, _hpP }, indexShift,false,true);
            con.Dispose();
            con = null;
            return ans;
        }

        /// <summary>
        /// conditionsを簡単な構文解析する.
        /// </summary>
        /// <param name="ints">条件文の左辺になる値の集合</param>
        /// <param name="labelIndexShift">labelの集合のこの番号から使う</param>
        /// <param name="readpast">一区切りの条件文を読み飛ばすかどうか,trueで読み飛ばす</param>
        /// <returns></returns>
        protected virtual bool analyze (int[] ints, int labelIndexShift = 0, bool readpast=false,bool theFirst=false)
        {
            bool ans=false;
            if(conditions==null)
                return true;
            int nowValue = 0;
            int nowIndex = 0;//今、比較に使われるはずのints[]の引数番号. これはlabelからの指定の時だけ修正するのがいいでしょう。
            while (pos < conditions.Length)
            {
                //Console.Write(conditions[pos]);
                switch (conditions[pos])
                {
                    case ' ':
                        break;
                    case '(':
                        addPos();// '('を読んだことにする
                        // '('があるから、')'までが一区切りで一つのboolが帰ってくるので,
                        // analyze()してその区切りを一つの条件文として読み取る
                        ans = analyze(ints, labelIndexShift, readpast);
                        break;
                    case ')':
                        addPos();// ')'を読んだことにする
                        //一つの区切りの終わりなので値を返す。 これは'('とセットになっているはずで、
                        if (theFirst && pos < conditions.Length) { //'('によって一区切りになっていない かつ ')'が文の最後ではない時
                            // つまり、')'が一つ余った時、エラーを出力する
                            Console.WriteLine("Condition Analysis Error:  extra ) before the last char of string!");
                            Console.WriteLine(conditions);
                            Console.WriteLine("return answer:" + ans);
                        }
                        return ans;
                    case '&':
                        addPos();// '&'を読んだことにする、これからも"addPos();"は"読んだことにする"と意味する

                        // and 条件に対して、すでにansがfalseなら、その後の条件は読まなくても答えはfalseと決まった
                        if (ans == false) analyze(null, 0, true);//ansと=しないことで、ただ読み飛ばすになる
                        else ans = analyze(ints, labelIndexShift, readpast);//readpastを付けるのは、読み飛ばし状態でこれに入っても読み飛ばしである。
                        break;
                    case '|':
                        addPos();
                        if (ans == true) analyze(null, 0, true);
                        else ans = analyze(ints, labelIndexShift, readpast);//readpastを付けるのは、読み飛ばし状態でこれに入っても読み飛ばしである。
                        break;
                    case '<':
                        addPos();
                        if (pos < conditions.Length && conditions[pos]=='='){
                            addPos();
                            ans = compareIntsWithInt(nowValue, smallerOrEqaul);
                            //Console.WriteLine("##" + ans + "##");
                        }
                        else
                        {
                            ans = compareIntsWithInt(nowValue, smallerThan);
                            //Console.WriteLine("##smaller Than" + ans + "##");
                        }
                        break;
                    case '>':
                        addPos();
                        if (pos + 1 < conditions.Length && conditions[pos] == '=')
                        {
                            addPos();
                            ans = compareIntsWithInt(nowValue, biggerOrEqaul);
                        }
                        else
                        {
                            ans = compareIntsWithInt(nowValue, biggerThan);
                        }
                        break;
                    case '=':
                        addPos();
                        ans = compareIntsWithInt(nowValue, equalWith);
                        //Console.WriteLine("## Equal" + ans + "##");
                        break;
                    
                    default:
                        // label を読もうとしている
                        for(int i = 0; i < label.Length-labelIndexShift; i++)
                        {
                            if (nextlabelIs(label[i+labelIndexShift]) ){
                                nowIndex = i;
                                break;
                            }
                        }
                        //ここでnowIndexを修正するといいでしょう.
                        if (ints==null || nowIndex < 0) nowIndex = 0;
                        else if (ints!=null && nowIndex >= ints.Length) nowIndex = ints.Length - 1;

                        if (ints != null) { nowValue = ints[nowIndex]; }
                        //Console.Write(ints[nowIndex]);
                        break;
                }
                addPos();
            }
            return ans;
        }
        protected void addPos()
        {
            if (pos < conditions.Length)
            {
                //Console.Write(conditions[pos]);
            }
            pos++;
            
        }
        /// <summary>
        /// ここは右辺が読み終わってその次になる
        /// </summary>
        /// <param name="left"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        protected virtual bool compareIntsWithInt(int left,Func< int, int, bool> compare)
        {
            //Console.Write("left:"+left);
            if (left == DataBase.motion_inftyTime) { left *= -1; }
            bool ans = compare(left, readOneInt());
            while (pos < conditions.Length)
            {
                if (conditions[pos] == ',')
                {
                    addPos();
                    if (ans == false)
                    {
                        ans = compare(left, readOneInt());
                    }
                    else { readOneInt(); }
                }else { pos--; break; }
            }
            return ans;
        }
        protected virtual bool equalWith(int left, int right) { //Console.WriteLine(left + "==" + right + " ; " + (left <= right));
            return left == right; }
        protected virtual bool biggerThan(int left,int right) { //Console.WriteLine(left + ">" + right + " ; " + (left <= right));
            return left > right; }
        protected virtual bool biggerOrEqaul(int left, int right) { //Console.WriteLine(left + ">=" + right + " ; " + (left <= right));
            return left >= right; }
        protected virtual bool smallerThan(int left,int right) {
            //Console.WriteLine(left + "<" + right + " ; " + (left <= right));
            return left < right; }
        protected virtual bool smallerOrEqaul(int left, int right) {
            //Console.WriteLine(left + "<=" + right + " ; " + (left <= right));
            return left <= right; }

        /// <summary>
        /// これが実行されたら数字の次である
        /// </summary>
        /// <returns></returns>
        protected virtual int readOneInt()
        {
            int ansI = 0;
            while (pos < conditions.Length)
            {
                if (conditions[pos]>='0' && conditions[pos] <= '9')
                {
                    ansI = ansI * 10 + conditions[pos] - '0';
                }else if( conditions[pos]!= ' ')
                {
                    break;
                }
                addPos();

            }
            //Console.Write("right:"+ansI);
            return ansI;
        }

        public static bool FromIndexStartsWith(string str, string startswith, int fromIndex)
        {
            if (fromIndex + startswith.Length > str.Length) return false;
            for(int i = 0; i < startswith.Length; i++)
            {
                if (startswith[i] != str[fromIndex + i]) return false;
            }
            return true;
        }
        protected bool nextlabelIs(string l)
        {
            if (pos + l.Length > conditions.Length) return false;
            for (int i = 0; i < l.Length; i++)
            {
                if (conditions[pos+i] != l[i]) return false;
            }
            pos += l.Length-1;
            return true;
        }
        protected Condition(string _c,int _p=0)
        {
            pos = _p;
            conditions = _c;
        }
        public void Dispose() {
            conditions = null;
        }
    }
}
